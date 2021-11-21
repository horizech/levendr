import React from 'react';
import { Link } from 'react-router-dom';
import { connect } from 'react-redux';
import { tablesActions } from '../actions';
import { Table } from 'reactstrap';
import { ButtonIcon } from '../components/button-icon.component';
import { history } from '../helpers';
import { Loading, Page, RolePermissions } from '../components';
import { CreateEditModal } from '../modals';
import { rolePermissionsService, permissionsService, rolesService } from '../services';
import { DialogModal } from '../modals';
const RolePermissionsPage = ({ match, location, dispatch, loggedIn }) => {

    const [rolePermissions, setRolePermissions] = React.useState(null);
    const [loadingRolePermissionColumns, setLoadingRolePermissionColumns] = React.useState(true);
    const [isCreateRolePermissionModalVisible, setCreateRolePermissionModalVisible] = React.useState(false);
    const [currentRow, setCurrentRow] = React.useState(null);
    const [isDeleteModalVisible, setDeleteModalVisible] = React.useState(false);
    const [isEditModalVisible, setEditModalVisible] = React.useState(false);
    const [deleteSuccess, setDeleteSuccess] = React.useState(false);
    const [addSuccess, setAddSuccess] = React.useState(false);
    const [updateSuccess, setUpdateSuccess] = React.useState(false);

    let selectOptionsList = [];
    const columns = [
        { Name: 'Id', value: 'Id' },
        { Name: 'Role', value: 'Role' },
        { Name: 'Permission', value: 'Permission' },
        { Name: 'CreatedOn', value: 'CreatedOn' },
        { Name: 'CreatedBy', value: 'CreatedBy' },
        { Name: 'LastUpdatedOn', value: 'LastUpdatedOn' },
        { Name: 'LastUpdatedBy', value: 'LastUpdatedBy' }
    ]
    const selectColumns = [
        { Name: 'Role' },
        { Name: 'Permission' }
    ]
    

    const showCreateModal = () => {
        setCreateRolePermissionModalVisible(true);
    }

    const handleOnCreateComplete = (values) => {
        if (values) {
            values.Role = values.Role.value;
            values.Permission = values.Permission.value;
            rolePermissionsService.addRolePermissions(values).then(response => {
                console.log(response);
                if (response.Success) {
                    setAddSuccess(true);
                }
                else {
                    setAddSuccess(false);
                }
            });
        }
        setCreateRolePermissionModalVisible(false);
    }
    const showEditModal = (row) => {
        setEditModalVisible(true);
        setCurrentRow(row);
    }

    const showDeleteConfirmationModal = (row) => {
        setDeleteModalVisible(true);
        setCurrentRow(row);
    }
console.log(currentRow);
    const handleOnEditComplete = (values) => {
        if (values) {
            values.Role = values.Role.value;
            values.Permission = values.Permission.value;
            rolePermissionsService.updateRolePermissions(values).then(response => {
                if (response.Success) {
                    setUpdateSuccess(true);
                }
                else {
                    setUpdateSuccess(false);
                }
            });
        }
        setEditModalVisible(false);
    }

    const handleOnDeleteComplete = (result) => {
        if (result === true) {
            rolePermissionsService.deleteRolePermissions(currentRow).then(response => {
                if (response.Success) {
                    setDeleteSuccess(true);
                }
                else {
                    setDeleteSuccess(false);
                }
            });
        }

        setDeleteModalVisible(false);
    }

    const buttonsConfig = [
        {
            label: "Delete",
            class: "btn-info",
            handle: () => handleOnDeleteComplete(true),
            closeOnClick: true
        },
        {
            label: "Cancel",
            class: "btn-secondary",
            handle: () => handleOnDeleteComplete(null),
            closeOnClick: true
        },
    ];
    const loadSelectOptions = () => {
        // const foreignSchema = (values.ForeignSchema.value);
        columns.map(column => {


            if (column.Name == selectColumns[1].Name) {
                permissionsService.getPermissions().then(
                    result => {
                        if (result.Success) {
                            // console.log(result.Data);
                            selectOptionsList[column.Name] = (
                                result.Data.map(x => {
                                    return { label: x.Name, value: x.Id }
                                }
                                )
                            );
                        }
                    }
                );
            }
            else if (column.Name == selectColumns[0].Name) {
                rolesService.getRoles().then(
                    result => {
                        if (result.Success) {
                            // console.log(result.Data);
                            selectOptionsList[column.Name] = (
                                result.Data.map(x => {
                                    return { label: x.Name, value: x.Id }
                                }
                                )
                            );
                        }
                    }
                );
            }
        });
    }
    if ((!selectOptionsList.Permission || (selectOptionsList.Permission && !selectOptionsList.Permission[0])) && (!selectOptionsList.Role || (selectOptionsList.Role && !selectOptionsList.Role[0])) ) {

        loadSelectOptions();
    }

    React.useEffect(() => {

       
        rolePermissionsService.getRolePermissions().then(response => {
            if (response.Success) {
                setRolePermissions(response.Data);
            }
            else {
                setRolePermissions(null);
            }
        })
        setLoadingRolePermissionColumns(false);
        if (!loggedIn) {
            history.push('/');
        }
        setAddSuccess(false);
        setDeleteSuccess(false);
        setUpdateSuccess(false);
    }, [match, loggedIn, deleteSuccess, addSuccess, updateSuccess]);
    return (
        <React.Fragment>
            <div align="right" style={{ marginBottom: "16px" }}>
                <button className="btn btn-primary" onClick={showCreateModal}>Create a new rolePermission</button>
            </div>
            {
                (loadingRolePermissionColumns) &&
                <Loading></Loading>
            }
            {
                (!loadingRolePermissionColumns && (!rolePermissions || rolePermissions["length"] == 0)) &&
                <div className="row">
                    <div className="col-sm-1 col-md-3"></div>
                    <div align="center" className="col-sm-10 col-md-6 col-md-offset-3" style={{ "marginTop": "25vh" }}>
                        <p>Nothing to show...
                        </p>
                    </div>
                    <div className="col-sm-1 col-md-3"></div>
                </div>
            }
            {
                (!loadingRolePermissionColumns && rolePermissions && rolePermissions["length"] != 0) &&
                <div>

                    <Table responsive bordered striped size="sm">
                        <thead>
                            <tr key={'header'}>
                                <th key={'header_#'} scope="col"></th>
                                {rolePermissions &&
                                    Object.keys(rolePermissions[0]).map(key => (
                                        <th key={'header_' + key} scope="col">{key}</th>
                                    ))
                                }
                            </tr>
                        </thead>
                        <tbody>
                            {rolePermissions &&
                                rolePermissions.map((row, i) => (
                                    <tr key={'row_' + (i + 1)}>

                                        <td key={'data_' + i + '_#'} scope="row">
                                            <div style={{ display: 'flex', flexDirection: 'row' }}>
                                                <ButtonIcon icon="edit" color="#007bff" onClick={() => showEditModal(row)} />
                                                <ButtonIcon icon="trash" color="#dc3545" onClick={() => showDeleteConfirmationModal(row)} />
                                            </div>
                                        </td>
                                        {
                                            Object.keys(rolePermissions[0]).map(key => (
                                                <td key={'data_' + i + key} >{row[key] != null ? '' + row[key] : ''}</td>
                                            ))
                                        }
                                    </tr>
                                ))
                            }
                        </tbody>
                    </Table>
                    {isEditModalVisible &&
                        <CreateEditModal
                            selectColumns={selectColumns}
                            selectOptionsList={selectOptionsList}
                            columns={columns}
                            row={currentRow}
                            handleOnClose={handleOnEditComplete}
                            mode="edit"
                            label="Edit" />
                    }
                    {isDeleteModalVisible &&

                        <DialogModal
                            headerLabel="Delete"
                            handleOnClose={handleOnDeleteComplete}
                            text="Do you really want to delete?"
                            buttonsConfig={buttonsConfig}
                        />

                    }
                </div>
            }
            {
                isCreateRolePermissionModalVisible && rolePermissions &&

                <CreateEditModal
                    selectColumns={selectColumns}
                    selectOptions={selectOptionsList}
                    columns={columns}
                    row={{}}
                    handleOnClose={handleOnCreateComplete}
                    mode="create"
                    buttonLabel="Create a new"
                />
            }
        </React.Fragment>
    );
}

function mapStateToProps(state) {
    const { loggedIn } = state.authentication;
    return {
        loggedIn,
    };
}

const connectedRolePermissions = connect(mapStateToProps)(RolePermissionsPage);
export { connectedRolePermissions as RolePermissionsPage };
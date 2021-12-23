import React from 'react';
import { Link } from 'react-router-dom';
import { connect } from 'react-redux';
import { tablesActions } from '../actions';
import { Table } from 'reactstrap';
import { ButtonIcon } from '../components/button-icon.component';
import { history } from '../helpers';
import { Loading, Page, PermissionGroupMappings } from '../components';
import { CreateEditModal } from '../modals';
import { permissionGroupMappingsService, rolesService } from '../services';
import { DialogModal } from '../modals';
import { LevendrTable } from '../components';
const PermissionGroupMappingsPage = ({ match, location, dispatch, loggedIn }) => {

    const [permissionGroupMappings, setPermissionGroupMappings] = React.useState(null);
    const [loadingPermissionGroupMappingColumns, setLoadingPermissionGroupMappingColumns] = React.useState(true);
    const [isCreatePermissionGroupMappingModalVisible, setCreatePermissionGroupMappingModalVisible] = React.useState(false);
    const [currentRow, setCurrentRow] = React.useState(null);
    const [isDeleteModalVisible, setDeleteModalVisible] = React.useState(false);
    const [isEditModalVisible, setEditModalVisible] = React.useState(false);
    const [deleteSuccess, setDeleteSuccess] = React.useState(false);
    const [addSuccess, setAddSuccess] = React.useState(false);
    const [updateSuccess, setUpdateSuccess] = React.useState(false);
    const [selectOptionsList, setSelectOptionsList] = React.useState({
        Permission: [],
        PermissionGroup: []
    });

    const columns = [
        { Name: 'Id', value: 'Id', Datatype: 'Integer' },
        { Name: 'Permission', value: 'Permission', Datatype: 'ShortText' },
        { Name: 'PermissionGroup', value: 'PermissionGroup', Datatype: 'LongText' },
        { Name: 'IsSystem', value: 'IsSystem', Datatype: 'Boolean' },
        { Name: 'CreatedOn', value: 'CreatedOn', Datatype: 'DateTime' },
        { Name: 'CreatedBy', value: 'CreatedBy', Datatype: 'ShortText' },
        { Name: 'LastUpdatedOn', value: 'LastUpdatedOn', Datatype: 'DateTime' },
        { Name: 'LastUpdatedBy', value: 'LastUpdatedBy', Datatype: 'ShortText' }
    ]

    const isSelectList =  {
        Permission: true,
        PermissionGroup: true
    };
    
    const showCreateModal = () => {
        setCreatePermissionGroupMappingModalVisible(true);
    }

    const handleOnCreateComplete = (values) => {
        if (values) {
            values.Permission = values.Permission.value;
            values.PermissionGroup = values.PermissionGroup.value;
            permissionGroupMappingsService.addPermissionGroupMapping(values).then(response => {
                console.log(response);
                if (response.Success) {
                    setAddSuccess(true);
                }
                else {
                    setAddSuccess(false);
                }
            });
        }
        setCreatePermissionGroupMappingModalVisible(false);
    }
    const showEditModal = (row) => {
        setEditModalVisible(true);
        console.log(selectOptionsList['Permission']);
        console.log(row['Permission']);
        console.log(selectOptionsList['PermissionGroup']);
        console.log(row['PermissionGroup']);
        if(row) {
            let editRow = JSON.parse(JSON.stringify(row));
            if(selectOptionsList['Permission'] && editRow['Permission']) {
                editRow['Permission'] = selectOptionsList['Permission'].filter(x => x.value == editRow['Permission'])[0];                             
            }
            if(selectOptionsList['PermissionGroup'] && editRow['PermissionGroup']) {
                editRow['PermissionGroup'] = selectOptionsList['PermissionGroup'].filter(x => x.value == editRow['PermissionGroup'])[0];                             
            }
            console.log(editRow);
            setCurrentRow(editRow);
        }
    }

    const showDeleteConfirmationModal = (row) => {
        setDeleteModalVisible(true);
        setCurrentRow(row);
    }
    
    console.log(currentRow);
    const handleOnEditComplete = (values) => {
        if (values) {
            values.Permission = values.Permission.value;
            values.PermissionGroup = values.PermissionGroup.value;
            console.log(values);
            permissionGroupMappingsService.updatePermissionGroupMapping(values).then(response => {
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
            permissionGroupMappingsService.deletePermissionGroupMapping(currentRow).then(response => {
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


            if (column.Name === 'PermissionGroup') {
                permissionGroupMappingsService.getPermissionGroupMappings().then(
                    result => {
                        if (result.Success) {
                            // console.log(result.Data);
                            let selectOptionsListUpdate = selectOptionsList;                            
                            selectOptionsListUpdate[column.Name] = (
                                result.Data.map(x => {
                                    return { label: x.Name, value: x.Id }
                                }
                                )
                            );
                            setSelectOptionsList(selectOptionsListUpdate);
                        }
                    }
                );
            }
            else if (column.Name ===  'Permission') {
                rolesService.getRoles().then(
                    result => {
                        if (result.Success) {
                            // console.log(result.Data);
                            let selectOptionsListUpdate = selectOptionsList;                            
                            selectOptionsListUpdate[column.Name] = (
                                result.Data.map(x => {
                                    return { label: x.Name, value: x.Id }
                                }
                                )
                            );
                            setSelectOptionsList(selectOptionsListUpdate);
                        }
                    }
                );
            }
        });
    }
    if ((!selectOptionsList.PermissionGroup || (selectOptionsList.PermissionGroup && !selectOptionsList.PermissionGroup[0])) && (!selectOptionsList.Permission || (selectOptionsList.Permission && !selectOptionsList.Permission[0])) ) {

        loadSelectOptions();
    }

    React.useEffect(() => {

       console.log(selectOptionsList);

        permissionGroupMappingsService.getPermissionGroupMappings().then(response => {
            if (response.Success) {
                setPermissionGroupMappings(response.Data);
            }
            else {
                setPermissionGroupMappings(null);
            }
        })
        setLoadingPermissionGroupMappingColumns(false);
        if (!loggedIn) {
            history.push('/');
        }
        setAddSuccess(false);
        setDeleteSuccess(false);
        setUpdateSuccess(false);
    }, [match, loggedIn, deleteSuccess, addSuccess, updateSuccess, selectOptionsList]);
    return (
        <React.Fragment>
            <div align="right" style={{ marginBottom: "16px" }}>
                <button className="btn btn-primary" onClick={showCreateModal}>Create a new permissionGroupMapping</button>
            </div>
            {
                (loadingPermissionGroupMappingColumns) &&
                <Loading></Loading>
            }
            {
                (!loadingPermissionGroupMappingColumns && (!permissionGroupMappings || permissionGroupMappings["length"] == 0)) &&
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
                (!loadingPermissionGroupMappingColumns && permissionGroupMappings && permissionGroupMappings["length"] != 0) &&
                <div>
                    <LevendrTable headers={Object.keys(permissionGroupMappings[0])}>
                    {permissionGroupMappings &&
                                permissionGroupMappings.map((row, i) => (
                                    <tr key={'row_' + (i + 1)}>

                                        <td key={'data_' + i + '_#'} scope="row">
                                            <div style={{ display: 'flex', flexDirection: 'row' }}>
                                                <ButtonIcon icon="edit" color="#007bff" onClick={() => showEditModal(row)} />
                                                <ButtonIcon icon="trash" color="#dc3545" onClick={() => showDeleteConfirmationModal(row)} />
                                            </div>
                                        </td>
                                        {
                                            Object.keys(permissionGroupMappings[0]).map(key => (
                                                <td key={'data_' + i + key} >{row[key] != null ? '' + row[key] : ''}</td>
                                            ))
                                        }
                                    </tr>
                                ))
                            }
                    </LevendrTable>
                    {/* <Table responsive bordered striped size="sm">
                        <thead>
                            <tr key={'header'}>
                                <th key={'header_#'} scope="col"></th>
                                {permissionGroupMappings &&
                                    Object.keys(permissionGroupMappings[0]).map(key => (
                                        <th key={'header_' + key} scope="col">{key}</th>
                                    ))
                                }
                            </tr>
                        </thead>
                        <tbody>
                            {permissionGroupMappings &&
                                permissionGroupMappings.map((row, i) => (
                                    <tr key={'row_' + (i + 1)}>

                                        <td key={'data_' + i + '_#'} scope="row">
                                            <div style={{ display: 'flex', flexDirection: 'row' }}>
                                                <ButtonIcon icon="edit" color="#007bff" onClick={() => showEditModal(row)} />
                                                <ButtonIcon icon="trash" color="#dc3545" onClick={() => showDeleteConfirmationModal(row)} />
                                            </div>
                                        </td>
                                        {
                                            Object.keys(permissionGroupMappings[0]).map(key => (
                                                <td key={'data_' + i + key} >{row[key] != null ? '' + row[key] : ''}</td>
                                            ))
                                        }
                                    </tr>
                                ))
                            }
                        </tbody>
                    </Table> */}
                    {isEditModalVisible &&
                        <CreateEditModal
                            isSelectList={isSelectList}
                            selectOptions={selectOptionsList}
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
                isCreatePermissionGroupMappingModalVisible && permissionGroupMappings &&

                <CreateEditModal
                    isSelectList={isSelectList}
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

const connectedPermissionGroupMappings = connect(mapStateToProps)(PermissionGroupMappingsPage);
export { connectedPermissionGroupMappings as PermissionGroupMappingsPage };

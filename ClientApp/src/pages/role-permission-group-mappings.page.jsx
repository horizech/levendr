import React from 'react';
import { connect } from 'react-redux';
import { ButtonIcon } from '../components/button-icon.component';
import { history } from '../helpers';
import { Loading } from '../components';
import { CreateEditModal } from '../modals';
import { rolePermissionGroupMappingsService, permissionGroupsService, rolesService } from '../services';
import { DialogModal } from '../modals';
import { LevendrTable } from '../components';
const RolePermissionGroupMappingsPage = ({ match, loggedIn }) => {

    const [rolePermissionGroupMappings, setRolePermissionGroupMappings] = React.useState(null);
    const [loadingRolePermissionGroupMappingColumns, setLoadingRolePermissionGroupMappingColumns] = React.useState(true);
    const [isCreateRolePermissionGroupMappingModalVisible, setCreateRolePermissionGroupMappingModalVisible] = React.useState(false);
    const [currentRow, setCurrentRow] = React.useState(null);
    const [isDeleteModalVisible, setDeleteModalVisible] = React.useState(false);
    const [isEditModalVisible, setEditModalVisible] = React.useState(false);
    const [deleteSuccess, setDeleteSuccess] = React.useState(false);
    const [addSuccess, setAddSuccess] = React.useState(false);
    const [updateSuccess, setUpdateSuccess] = React.useState(false);
    const [selectOptionsList, setSelectOptionsList] = React.useState({
        Role: [],
        PermissionGroup: []
    });

    const columns = [
        { Name: 'Id', value: 'Id', Datatype: 'Integer' },
        { Name: 'Role', value: 'Role', Datatype: 'ShortText' },
        { Name: 'PermissionGroup', value: 'PermissionGroup', Datatype: 'LongText' },
        { Name: 'IsSystem', value: 'IsSystem', Datatype: 'Boolean' },
        { Name: 'CreatedOn', value: 'CreatedOn', Datatype: 'DateTime' },
        { Name: 'CreatedBy', value: 'CreatedBy', Datatype: 'ShortText' },
        { Name: 'LastUpdatedOn', value: 'LastUpdatedOn', Datatype: 'DateTime' },
        { Name: 'LastUpdatedBy', value: 'LastUpdatedBy', Datatype: 'ShortText' }
    ]

    const isSelectList =  {
        Role: true,
        PermissionGroup: true
    };
    
    const showCreateModal = () => {
        setCreateRolePermissionGroupMappingModalVisible(true);
    }

    const handleOnCreateComplete = (values) => {
        if (values) {
            values.Role = values.Role.value;
            values.PermissionGroup = values.PermissionGroup.value;
            rolePermissionGroupMappingsService.addRolePermissionGroupMapping(values).then(response => {
                console.log(response);
                if (response.Success) {
                    setAddSuccess(true);
                }
                else {
                    setAddSuccess(false);
                }
            });
        }
        setCreateRolePermissionGroupMappingModalVisible(false);
    }
    const showEditModal = (row) => {
        setEditModalVisible(true);
        console.log(selectOptionsList['Role']);
        console.log(row['Role']);
        console.log(selectOptionsList['PermissionGroup']);
        console.log(row['PermissionGroup']);
        if(row) {
            let editRow = JSON.parse(JSON.stringify(row));
            if(selectOptionsList['Role'] && editRow['Role']) {
                editRow['Role'] = selectOptionsList['Role'].filter(x => x.value == editRow['Role'])[0];                             
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
            values.Role = values.Role.value;
            values.PermissionGroup = values.PermissionGroup.value;
            console.log(values);
            rolePermissionGroupMappingsService.updateRolePermissionGroupMapping(values).then(response => {
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
            rolePermissionGroupMappingsService.deleteRolePermissionGroupMapping(currentRow).then(response => {
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
        let permissionGroupsPromise = permissionGroupsService.getPermissionGroups();
        let rolesPromise = rolesService.getRoles();
        Promise.all([permissionGroupsPromise, rolesPromise]).then( data => {
            console.log('Promise.all result: ', data);
            let selectOptionsListUpdate = {};
            selectOptionsListUpdate['PermissionGroup'] = (
                data[0].Data.map(x => {
                    return { label: x.Name, value: x.Id }
                })
            );
            selectOptionsListUpdate['Role'] = (
                data[1].Data.map(x => {
                    return { label: x.Name, value: x.Id }
                })
            );
            setSelectOptionsList(selectOptionsListUpdate);
        })        
        // const foreignSchema = (values.ForeignSchema.value);        
    }


    React.useEffect( () => {
        if (!selectOptionsList.PermissionGroup || !selectOptionsList.PermissionGroup.length || !selectOptionsList.Role || !selectOptionsList.Role.length) {
            loadSelectOptions();
        }
    }, []);

    React.useEffect(() => {

       console.log(selectOptionsList);

        rolePermissionGroupMappingsService.getRolePermissionGroupMappings().then(response => {
            if (response.Success) {
                setRolePermissionGroupMappings(response.Data);
            }
            else {
                setRolePermissionGroupMappings(null);
            }
        })
        setLoadingRolePermissionGroupMappingColumns(false);
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
                <button className="btn btn-primary" onClick={showCreateModal}>Create a new rolePermissionGroupMapping</button>
            </div>
            {
                (loadingRolePermissionGroupMappingColumns) &&
                <Loading></Loading>
            }
            {
                (!loadingRolePermissionGroupMappingColumns && (!rolePermissionGroupMappings || rolePermissionGroupMappings["length"] == 0)) &&
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
                (!loadingRolePermissionGroupMappingColumns && rolePermissionGroupMappings && rolePermissionGroupMappings["length"] != 0) &&
                <div>
                    <LevendrTable headers={Object.keys(rolePermissionGroupMappings[0])}>
                    {rolePermissionGroupMappings &&
                                rolePermissionGroupMappings.map((row, i) => (
                                    <tr key={'row_' + (i + 1)}>

                                        <td key={'data_' + i + '_#'} scope="row">
                                            <div style={{ display: 'flex', flexDirection: 'row' }}>
                                                <ButtonIcon icon="edit" color="#007bff" onClick={() => showEditModal(row)} />
                                                <ButtonIcon icon="trash" color="#dc3545" onClick={() => showDeleteConfirmationModal(row)} />
                                            </div>
                                        </td>
                                        {
                                            Object.keys(rolePermissionGroupMappings[0]).map(key => (
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
                                {rolePermissionGroupMappings &&
                                    Object.keys(rolePermissionGroupMappings[0]).map(key => (
                                        <th key={'header_' + key} scope="col">{key}</th>
                                    ))
                                }
                            </tr>
                        </thead>
                        <tbody>
                            {rolePermissionGroupMappings &&
                                rolePermissionGroupMappings.map((row, i) => (
                                    <tr key={'row_' + (i + 1)}>

                                        <td key={'data_' + i + '_#'} scope="row">
                                            <div style={{ display: 'flex', flexDirection: 'row' }}>
                                                <ButtonIcon icon="edit" color="#007bff" onClick={() => showEditModal(row)} />
                                                <ButtonIcon icon="trash" color="#dc3545" onClick={() => showDeleteConfirmationModal(row)} />
                                            </div>
                                        </td>
                                        {
                                            Object.keys(rolePermissionGroupMappings[0]).map(key => (
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
                isCreateRolePermissionGroupMappingModalVisible && rolePermissionGroupMappings &&

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

const connectedRolePermissionGroupMappings = connect(mapStateToProps)(RolePermissionGroupMappingsPage);
export { connectedRolePermissionGroupMappings as RolePermissionGroupMappingsPage };

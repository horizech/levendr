import React from 'react';
import { Link } from 'react-router-dom';
import { connect } from 'react-redux';
import { tablesActions } from '../actions';
import { Table } from 'reactstrap';
import { ButtonIcon } from '../components/button-icon.component';
import { history } from '../helpers';
import { Loading, Page, User } from '../components';
import { CreateEditModal } from '../modals';
import { userService } from '../services';
import { DialogModal } from '../modals'
const UsersPage = ({ match, location, dispatch, user, loggedIn }) => {

    const [loadingSettingColumns, setLoadingSettingColumns] = React.useState(true);
    const [isCreateSettingModalVisible, setCreateSettingModalVisible] = React.useState(false);
    const [currentRow, setCurrentRow] = React.useState(null);
    const [isDeleteModalVisible, setDeleteModalVisible] = React.useState(false);
    const [isEditModalVisible, setEditModalVisible] = React.useState(false);
    const [deleteSuccess, setDeleteSuccess] = React.useState(false);
    const [addSuccess, setAddSuccess] = React.useState(false);
    const [updateSuccess, setUpdateSuccess] = React.useState(false);
    const [users, setUsers] = React.useState([user]);
    
    const columns = [
        { Name: 'Id', value: 'Id' },
        { Name: 'Username', value: 'Username' },
        { Name: 'Email', value: 'Email' },
        { Name: 'Fullname', value: 'Fullname' },
        //{ Name: 'Token', value: 'Token' },
        //{ Name: 'Role', value: 'Role' },
        //{ Name: 'Permissions', value: 'Permissions' },
        { Name: 'CreatedOn', value: 'CreatedOn' },
        { Name: 'LastUpdatedOn', value: 'LastUpdatedOn' }
    ]

    const displayedColumns = columns.map( x => x.Name);
    
    const showCreateModal = () => {
        setCreateSettingModalVisible(true);
    }
    const handleOnCreateComplete = (values) => {
        if (values) {
            userService.registerUser(values).then(response => {

                if (response.Success) {
                    setAddSuccess(true);

                }
                else {

                    setAddSuccess(false);
                }
            });
        }
        setCreateSettingModalVisible(false);
    }
    const showEditModal = (row) => {
        setEditModalVisible(true);
        setCurrentRow(row);
    }

    const showDeleteConfirmationModal = (row) => {
        setDeleteModalVisible(true);
        setCurrentRow(row);
    }

    const handleOnEditComplete = (values) => {
        if (values) {
            userService.updateUser(currentRow.Key, values).then(response => {
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
            userService.deleteUser(currentRow).then(response => {
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

    
    React.useEffect(() => {

        // userService.getAllUsers().then( response => {
        //     console.log(response);
        //     if(response && response.Success) {
        //         setUser(response.Data);        
        //     }
        //     else {
        //         setUser(null);
        //     }
        // })       
        // setUser(null);
        setLoadingSettingColumns(false);
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
                <button className="btn btn-primary" onClick={showCreateModal}>Create a new User</button>
            </div>
            {
                (loadingSettingColumns) &&
                <Loading></Loading>
            }
            {
                (!loadingSettingColumns && (!users || users.length == 0)) &&
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
                (!loadingSettingColumns && users && users.length > 0) &&
                <div>

                    <Table responsive bordered striped size="sm">
                        <thead>
                            <tr key={'header'}>
                                <th key={'header_#'} scope="col"></th>
                                {displayedColumns &&
                                    displayedColumns.map(key => (
                                        <th key={'header_' + key} scope="col">{key}</th>
                                    ))                                    
                                }
                                <th key={'header_Role'} scope="col">Role</th>

                            </tr>
                        </thead>
                        <tbody>
                            {users &&
                                users.map((row, i) => (
                                    <tr key={'row_' + (i + 1)}>

                                        <td key={'data_' + i + '_#'} scope="row">
                                            <div style={{ display: 'flex', flexDirection: 'row' }}>
                                                <ButtonIcon icon="edit" color="#007bff" onClick={() => showEditModal(row)} />
                                                <ButtonIcon icon="trash" color="#dc3545" onClick={() => showDeleteConfirmationModal(row)} />
                                            </div>
                                        </td>                                        
                                        {
                                            displayedColumns.map(key => (
                                                <td key={'data_' + i + key} >{row[key] != null ? '' + row[key] : ''}</td>
                                            ))                                            
                                        }
                                        <td key={'data_' + i + 'Role'} >{row['Role'] != null ? row['Role']['Name'] : ''}</td>
                                        
                                    </tr>
                                ))
                            }
                        </tbody>
                    </Table>
                    {isEditModalVisible &&
                        <CreateEditModal
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
                isCreateSettingModalVisible &&
                <CreateEditModal
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
    const { loggedIn, user } = state.authentication;
    return {
        loggedIn,
        user
    };
}

const connectedUser = connect(mapStateToProps)(UsersPage);
export { connectedUser as UsersPage };
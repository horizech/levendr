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
import {
    Card, CardImg, CardBody, CardColumns, Collapse, UncontrolledCollapse,
    CardTitle, CardText, Button
} from "reactstrap"

const UserPage = ({ match, location, dispatch, user, loggedIn }) => {

    // const [user, setUser] = React.useState(null);
    const [loadingSettingColumns, setLoadingSettingColumns] = React.useState(true);
    const [isCreateSettingModalVisible, setCreateSettingModalVisible] = React.useState(false);
    const [currentRow, setCurrentRow] = React.useState(null);
    const [isDeleteModalVisible, setDeleteModalVisible] = React.useState(false);
    const [isEditModalVisible, setEditModalVisible] = React.useState(false);
    const [deleteSuccess, setDeleteSuccess] = React.useState(false);
    const [addSuccess, setAddSuccess] = React.useState(false);
    const [updateSuccess, setUpdateSuccess] = React.useState(false);

    const columns = [
        { Name: 'Id', value: 'Id' },
        { Name: 'Username', value: 'Username' },
        { Name: 'Email', value: 'Email' },
        { Name: 'Fullname', value: 'Fullname' },
        { Name: 'Token', value: 'Token' },
        { Name: 'Role', value: 'Role' },
        { Name: 'Permissions', value: 'Permissions' },
        { Name: 'CreatedOn', value: 'CreatedOn' },
        { Name: 'LastUpdatedOn', value: 'LastUpdatedOn' }
    ]

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

    // console.log(user);


    React.useEffect(() => {


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


            <div className="col-sm-12">
                <Button
                    color="primary"
                    id="togglerUserInfo"
                    style={{
                        marginBottom: '1rem'
                    }}
                >
                    User Info
                </Button>
                <UncontrolledCollapse toggler="#togglerUserInfo">
                    <Card>
                        <CardBody>
                            <div >User Name: {user.Username}</div>
                            <div >Full Name: {user.Fullname}</div>
                            <div >Email: {user.email}</div>
                        </CardBody>
                    </Card>
                </UncontrolledCollapse>
            </div>

            {/* <CardColumns></CardColumns> */}
        
                
                <div className="col-md-6">
        
                    <Card>
                    <CardTitle color="primary"
                               tag="h3"
                                id="togglerUserRole"
                                style={{
                                    marginTop: '1rem',
                                    marginBottom: '1rem',
                                    textAlign: "center"
                                }}> 
                                User Role
                                </CardTitle>
                    <UncontrolledCollapse toggler="#togglerUserRole">
                        
                            
                            <CardBody style={{
                                    
                                    textAlign: "center"
                                }}>
                                <div >{user.Role.Description}</div>
                            </CardBody>
                        
                    </UncontrolledCollapse>
                    </Card>
                </div>


                <div className="col-md-6">
        
                    <Card>
                    <CardTitle color="primary"
                               tag="h3"
                                id="togglerUserPermissions"
                                style={{
                                    marginTop: '1rem',
                                    marginBottom: '1rem',
                                    textAlign: "center"
                                }}> 
                                User Permissions
                                </CardTitle>
                    <UncontrolledCollapse toggler="#togglerUserPermissions">
                        
                            
                            <CardBody style={{
                                    
                                    textAlign: "center"
                                }}>
                                <div >{user && user.Permissions && user.Permissions.map((permission, i) => <div key= {i}>{permission.Description}</div>)}</div>
                            </CardBody>
                        
                    </UncontrolledCollapse>
                    </Card>
                </div>

          










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

const connectedUser = connect(mapStateToProps)(UserPage);
export { connectedUser as UserPage };
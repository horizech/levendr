import React from 'react';
import { connect } from 'react-redux';
import { tablesActions } from '../actions';
import { ButtonIcon } from '../components/button-icon.component';
import { history } from '../helpers';
import {
    Table, Card, CardImg, CardBody, CardColumns, Collapse, UncontrolledCollapse,
    CardTitle, CardText, Button, CardHeader,
    CardFooter, Row, Col
} from "reactstrap";
import { Collapsible } from '../components';

const UserPage = ({ user, loggedIn }) => {

    // console.log(user);


    React.useEffect(() => {
        if (!loggedIn) {
            history.push('/');
        }
    }, [loggedIn]);

    return (
        <React.Fragment>
            <Collapsible icon="hello" title="test" subTitle="Sub Text">
                <p>Hello world! How are you?</p>
            </Collapsible>
            <Card style={{
                marginTop: '1rem',
                marginBottom: '1rem',
            }}>
                <CardTitle
                    tag="h5"
                    id="togglerUserInfo"
                    style={{
                        marginTop: '1rem',
                        marginBottom: '1rem',
                        paddingLeft: '20px'
                    }}>User Info
                </CardTitle>
                <CardBody >
                    <div
                        style={{
                            display: "flex",
                            flexFlow: "row nowrap",
                            width: "auto",
                            height: "auto",
                            borderTop: "1px solid #dfdfdf",
                            boxSizing: "border-box",
                            paddingTop: "15px",
                            paddingBottom: "15px",
                        }}>

                        <div className="col-sm-6">User Name: </div>
                        <div className="col-sm-6">{user.Username}</div>



                    </div>
                    <div style={{
                        display: "flex",
                        flexFlow: "row nowrap",
                        width: "auto",
                        height: "auto",
                        borderTop: "1px solid #dfdfdf",
                        boxSizing: "border-box",
                        paddingTop: "15px",
                        paddingBottom: "15px",
                    }}>

                        <div className="col-sm-6">Full Name: </div>
                        <div className="col-sm-6">{user.Fullname}</div>

                    </div>
                    <div style={{
                        display: "flex",
                        flexFlow: "row nowrap",
                        width: "auto",
                        height: "auto",
                        borderTop: "1px solid #dfdfdf",
                        paddingTop: "15px",
                        paddingBottom: "15px",
                        boxSizing: "border-box",
                    }}>

                        <div className="col-sm-6">Email: </div>
                        <div className="col-sm-6">{user.email}</div>
                    </div>
                </CardBody>
            </Card>

            <Card
                style={{
                    marginTop: '1rem',
                    marginBottom: '1rem',
                }}>
                <CardTitle color="primary"
                    tag="h5"
                    id="togglerUserRole"
                    style={{
                        marginTop: '1rem',
                        marginBottom: '1rem',
                        paddingLeft: '20px'

                    }}>
                    User Role
                </CardTitle>
                <UncontrolledCollapse toggler="#togglerUserRole">


                    <CardBody style={{

                        textAlign: "center",
                    }}>
                        <div style={{
                            borderTop: "1px solid #dfdfdf",
                            padding: "15px",

                        }}>{user.Role.Description}</div>
                    </CardBody>

                </UncontrolledCollapse>
            </Card>

            <Card
                style={{
                    marginTop: '1rem',
                    marginBottom: '1rem',
                }}>
                <CardTitle color="primary"
                    tag="h5"
                    id="togglerUserPermissions"
                    style={{
                        marginTop: '1rem',
                        marginBottom: '1rem',
                        paddingLeft: '20px'
                    }}>
                    User Permissions
                </CardTitle>
                <UncontrolledCollapse toggler="#togglerUserPermissions">
                    <CardBody>
                        <Table responsive bordered striped size="md">

                            <tbody>
                                {user &&
                                    user.Permissions && user.Permissions[0].Description && user.Permissions.map((row, i) => (
                                        <tr key={i}>{'' + row.Description}</tr>
                                    ))
                                }
                            </tbody>
                        </Table>
                    </CardBody>

                </UncontrolledCollapse>
            </Card>

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
import React from 'react';
import { connect } from 'react-redux';
import { tablesActions } from '../actions';
import { ButtonIcon } from '../components/button-icon.component';
import { history } from '../helpers';
import {
    Table, Card, CardImg, CardBody, CardColumns, Collapse, UncontrolledCollapse,
    CardTitle, CardText, Button, CardHeader, ListGroup, ListGroupItem,
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
            <Collapsible style={{marginTop: "32px"}} icon="user" title="User Info" subTitle="" defaultOpen={true}>
                <ListGroup variant="flush">
                    <ListGroupItem>
                        <div className="row">
                            <div className={"col-md-4"}>Username:</div>
                            <div className={"col-md-8"}>{user.Username}</div>
                        </div>
                    </ListGroupItem>
                    <ListGroupItem>   
                        <div className="row">
                            <div className={"col-md-4"}>Full Name:</div>
                            <div className={"col-md-8"}>{user.Fullname}</div>
                        </div>
                    </ListGroupItem>
                    <ListGroupItem>
                    <div className="row">
                        <div className={"col-md-4"}>Email:</div>
                        <div className={"col-md-8"}>{user.email}</div>
                    </div>
                </ListGroupItem>
               
            </ListGroup>
            </Collapsible>
            
            <Collapsible style={{marginTop: "32px"}} icon="user-tag" title=" User Role" subTitle="">
                <ListGroup variant="flush">
                    <ListGroupItem>{user.Role.Description}</ListGroupItem>
                   
                </ListGroup>
            </Collapsible>
            <Collapsible style={{marginTop: "32px"}}  icon="user" title=" User Permissions" subTitle="">
                <ListGroup variant="flush">
                {
                    user && user.Permissions && user.Permissions.length > 0 && user.Permissions.map((row, i) => (
                        <ListGroupItem key={'permission-' + i}>{'' + row.Description}</ListGroupItem>
                    ))
                }
                </ListGroup>
            </Collapsible>

     
        
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
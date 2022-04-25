import React from 'react';
import { connect } from 'react-redux';
import { Card, Button, CardTitle, CardText, Row, Col,CardImg } from 'reactstrap';
import { history } from '../helpers';

const ConfigurationPage = (props) => {
  return (
    <Row>
      <Col sm="4">
        <Card body inverse style={{ borderColor: '#7a77b9' }}  color="white">
        <center>
          {/* <CardTitle>Roles</CardTitle><br/> */}
          <i class="fas fa-user-tag fa-10x center" style={{color: '#7a77b9'}}></i>
        </center>
        
          <CardText></CardText>
          <Button  onClick={ () => history.push("/roles")} color='primary'>Manage Roles</Button>
        </Card>
      </Col>
      <Col sm="4">
        <Card body inverse style={{ borderColor: '#7a77b9' }}  color="white">
        <center>
          <i class="fas fa-user-lock fa-10x center" style={{color: '#7a77b9'}}></i>
        </center>
        
          <CardText></CardText>
          <Button  onClick={ () => history.push("/permissions")} color='primary'>Permissions</Button>
        </Card>
      </Col>
      <Col sm="4">
        <Card body inverse style={{ borderColor: '#7a77b9' }} color="white">
        <center>    
          <i class="fas fa-layer-group fa-10x " style={{color: '#7a77b9'}}></i>
        </center>
     
          <CardText></CardText>
          <Button  onClick={ () => history.push("/permission-groups")} color='primary'>Permisssion Group </Button>
        </Card>
      </Col>
      <Col sm="4">
        <Card body inverse style={{borderColor: '#7a77b9',marginTop:"10px" }}  color="white">
          <center>    
            <i class="fas fa-users-cog fa-10x center" style={{color: '#7a77b9'}}></i>
          </center>
          
          <CardText></CardText>
          <Button  onClick={ () => history.push("/permission-group-mappings")} color='primary'>Permisssion Group Mappings</Button>
        </Card>
      </Col>
      <Col sm="4">
        <Card body inverse style={{ borderColor: '#7a77b9',marginTop:"10px" }}  color="white">
          <center>    
            <i class="fas fa-sitemap fa-10x center" style={{color: '#7a77b9'}}></i>
          </center>
          {/* <CardTitle></CardTitle> */}
         
          <CardText></CardText>
          <Button  onClick={ () => history.push("/role-permission-group-mappings")} color='primary'>Role Permisssion Group Mappings</Button>
        </Card>
      </Col>
    </Row>
  );
};

function mapStateToProps(state) {
  const { loggedIn } = state.authentication;
  return {
      loggedIn
  };
}
const connectedHome = connect(mapStateToProps)(ConfigurationPage);
 export { connectedHome as ConfigurationPage }; 

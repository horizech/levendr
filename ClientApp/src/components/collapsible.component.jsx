import React, { Component } from 'react';
import { Container, Card, CardBody, CardHeader, CardTitle, CardSubtitle, CardText } from 'reactstrap';
import { NavMenu } from './nav-menu.component';
import { Sidebar } from './sidebar.component';

import '../styles/Collapsible.css';

export const Collapsible = ({title, subTitle, icon, defaultOpen, children}) => {
  
  const [isOpen, setOpen] = React.useState(defaultOpen || false);

  const toggle = () => {
    setOpen(!isOpen);
  }

  return (
    <React.Fragment>
      <Card>
        <CardHeader onClick={toggle}>
          <div style={{display: 'flex', alignItems: 'center'}}>
            <div style={{marginRight: '16px'}}>
              {icon}
            </div>
            <div style={{flex: '1'}}>
              <CardTitle>{title}</CardTitle>
              <CardSubtitle className="mb-2 text-muted">{subTitle}</CardSubtitle>        
            </div>
            <div style={{marginLeft: '16px'}}>
              {icon}
            </div>
          </div>
        </CardHeader>        
          <CardBody className={isOpen? 'open': 'collapsed'}>
            <CardText>
            {
              isOpen &&
              children
            }
          </CardText>
          </CardBody>
        </Card>      
    </React.Fragment>
  );
}

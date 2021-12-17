import React, { Component } from "react";
import {
  Collapse,
  Container,
  Navbar,
  NavbarBrand,
  NavbarToggler,
  NavItem,
  NavLink,
} from "reactstrap";
import { Link } from "react-router-dom";
import { connect } from 'react-redux';

import "../styles/NavMenu.scss";

const NavMenu = ({loggedIn}) => {
  const displayName = NavMenu.name;
  const [ collapsed, setCollapsed ] = React.useState(true);
  const toggleNavbar = () => {
    setCollapsed(!collapsed);
  }

  return  (
    <header>
      {
        loggedIn &&
        <Navbar
          className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3 bg-primary levendr-navbar"
          dark
        >
          <Container>
            <NavbarBrand tag={Link} to="/">
              Levendr
            </NavbarBrand>
            <NavbarToggler onClick={toggleNavbar} className="mr-2" />
            <Collapse
              className="d-sm-inline-flex flex-sm-row-reverse"
              isOpen={!collapsed}
              navbar
            >
              <ul className="navbar-nav flex-grow">
                <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/">
                    Home
                  </NavLink>
                </NavItem>
                <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/admin">
                    Admin
                  </NavLink>
                </NavItem>
                <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/counter">
                    Counter
                  </NavLink>
                </NavItem>
                <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/fetch-data">
                    Fetch data
                  </NavLink>
                </NavItem>
                <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/logout">
                    Logout
                  </NavLink>
                </NavItem>
              </ul>
            </Collapse>
          </Container>
        </Navbar>
      }
    </header>
  );

}

function mapStateToProps(state) {
    const { loggedIn } = state.authentication;
    return {
        loggedIn
    };
}

const connectedNavMenu = connect(mapStateToProps)(NavMenu);
export { connectedNavMenu as NavMenu }; 
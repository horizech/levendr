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
// import { NavSubMenu } from 'components';
import "../styles/NavMenu.scss";
import { LevendrDropdown, TableDropdown } from ".";
import { history } from '../helpers';

const NavMenu = ({ loggedIn, user, tableslist, loadingTablesList }) => {
  // const displayName = NavMenu.name;
  const [collapsed, setCollapsed] = React.useState(true);
  const toggleNavbar = () => {
    setCollapsed(!collapsed);
  }
  let permissionGroupsNames = [];
  if (user !== null && user.PermissionGroups) {
    permissionGroupsNames = user.PermissionGroups.map((x) => {
      return (x.Name);
    })

  }
  console.log(permissionGroupsNames.includes("UsersReadWrite"));
  console.log(permissionGroupsNames);
  let permissionItems = [
    { name: "Permissions", path: "/permissions", permissionGroup: "PermissionsReadWrite" },
    { name: "Permission Groups", path: "/permission-groups", permissionGroup: "PermissionGroupsReadWrite" },
    { name: "Permission Group Mappings", path: "/permission-group-mappings", permissionGroup: "PermissionGroupMappingsReadWrite" },
    { name: "Role Permission Group Mappings", path: "/role-permission-group-mappings", permissionGroup: "RolePermissionGroupMappingsReadWrite" },
  ]
  let filteredPermissionItems = [];
  filteredPermissionItems = permissionItems.filter((item) => {
    return permissionGroupsNames.includes(item.permissionGroup);
  });
  console.log(filteredPermissionItems);

  
const renderLoading = () =>  {
  return (
    <NavItem>Loading...</NavItem>
  );
}

  return (
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
                {/* <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/admin">
                    Admin
                  </NavLink>
                </NavItem> */}
                <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/user">
                    Current User
                  </NavLink>
                </NavItem>
                {
                  permissionGroupsNames.includes("UsersReadWrite") &&
                  <NavItem>
                    <NavLink tag={Link} className="text-dark" to="/users">
                      Users
                    </NavLink>
                  </NavItem>
                }
                {
                  permissionGroupsNames.includes("SettingsReadWrite") &&
                  <NavItem>
                    <NavLink tag={Link} className="text-dark" to="/settings">
                      Preferences
                    </NavLink>
                  </NavItem>
                }
                {
                  filteredPermissionItems.length > 0 &&
                  <LevendrDropdown title= "Permissions" items={filteredPermissionItems} onClick={ (path) => history.push(path)}/>
                }
                {
                    (loadingTablesList === false && tableslist !== null)?
                    <TableDropdown title= "Tables" items={tableslist} onClick={ (path) => history.push(path)}/>
                    :renderLoading()
                }
                {
                  permissionGroupsNames.includes("RolesReadWrite") &&
                  <NavItem>
                    <NavLink tag={Link} className="text-dark" to="/roles">
                      Roles
                    </NavLink>
                  </NavItem>
                }
                {
                  permissionGroupsNames.includes("RolesReadWrite") &&
                  <NavItem>
                    <NavLink tag={Link} className="text-dark" to="/configuration">
                      Configuration
                    </NavLink>
                  </NavItem>
                }
                {/* <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/counter">
                    Counter
                  </NavLink>
                </NavItem>  */}
                {/* <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/fetch-data">
                    Fetch data
                  </NavLink>
                </NavItem> */}
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
  const { user, loggedIn } = state.authentication;
  const { tableslist, loadingTablesList} = state.tables

  return {
    user,
    loggedIn,
    tableslist, 
    loadingTablesList
  };
}

const connectedNavMenu = connect(mapStateToProps)(NavMenu);
export { connectedNavMenu as NavMenu }; 
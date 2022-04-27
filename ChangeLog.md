# ChangeLog:

## Unreleased
### Added:
### Changed:
- Renamed UserLevelAccess table to UserAccessLevels.
### Deprecated:
### Removed:
### Fixed:
### Security:


## v0.1.1 - 2022-04-25
### Added:
- Added PermissionGroups in GetUserInfo result.
- Use LevendrUserAccessLevel filter in Table controller functions.
- Added LevendrUserAccessLevel filter.
- Added boolen type output in Execute<T> function.
- Implemented MemoryCache in many services.
- Added MemoryCache system.
- Connected UserLevelAccess with RolePermissionGroup
- Added UserLevelAccess table.
- Added error and success alerts and toasts
- Implemented Alerts
- Added title to alert
- Implemented parseInt in create-edit public tables
- Levendr Table create-edit-modal: implemented parseInt & updated dropdown
- Implemented addUser FE
- Implemented AddUser API
- Implemened Levendr Dropdown component
### Changed:
- Updated NavMenu Component.
- Updated Table controller to use UserAccessLevel
- Updated UpdateRows function.
- Updated Permissions Configuration.
- Updated LevendrAuthorized filter
- Updated RolePermission in JWT and authorization.
- Added tables to NavBar and removed sidebar
- Updated navmenu with role, permissions, settings and users
- Updated Login response.
- Code cleanup
### Deprecated:
### Removed:
### Fixed:
- Fixed AddUser API
### Security:


## v0.1.0 - 2022-01-11
- First release
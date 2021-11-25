import { UserApiProvider } from '../api_providers';

export const userService = {
    login,
    logout,
    registerUser,
    getAllUsers,
    getUserById,
    updateUser,
    deleteUser
};

function login(Username, Password) {
    return UserApiProvider.login(Username, Password);
}

function logout() {
    return UserApiProvider.logout();
}

function getAllUsers() {
    return UserApiProvider.getAllUsers();
}

function getUserById(id) {
    return UserApiProvider.getUserById(id);
}

function registerUser(user) {
    return UserApiProvider.registerUser(user);
}

function updateUser(user) {
    return UserApiProvider.updateUser(user);
}

// prefixed function name with underscore because delete is a reserved word in javascript
function deleteUser(id) {
    return UserApiProvider.deleteUser(id)
}

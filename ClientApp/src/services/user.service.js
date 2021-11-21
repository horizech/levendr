import { UserApiProvider } from '../api_providers';

export const userService = {
    login,
    logout,
    register,
    getAll,
    getById,
    update,
    delete: _delete
};

function login(Username, Password) {
    return UserApiProvider.login(Username, Password);
}

function logout() {
    return UserApiProvider.logout();
}

function getAll() {
    return UserApiProvider.getAll();
}

function getById(id) {
    return UserApiProvider.getById(id);
}

function register(user) {
    return UserApiProvider.register(user);
}

function update(user) {
    return UserApiProvider.update(user);
}

// prefixed function name with underscore because delete is a reserved word in javascript
function _delete(id) {
    return UserApiProvider.delete(id)
}

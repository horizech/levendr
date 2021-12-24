import { authHeader, config } from '../helpers';
import { handleError, handleResponse } from './handler.api';


class UserApiProvider {
    static async login(Username, Password) {
        const requestOptions = {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ Username, Password })
        };

        return fetch(config.apiUrl + '/api/user/login', requestOptions)
            .then(handleResponse, handleError)
            .then(result => {
                return result;
            });
    }

    static logout() {
        // remove user from local storage to log user out
        localStorage.removeItem('user');
    }

    static getAllUsers() {
        const requestOptions = {
            method: 'GET',
            headers: authHeader()
        };

        return fetch(config.apiUrl + '/users', requestOptions).then(handleResponse, handleError);
    }

    static getUserById(id) {
        const requestOptions = {
            method: 'GET',
            headers: authHeader()
        };

        return fetch(config.apiUrl + '/users/' + id, requestOptions).then(handleResponse, handleError);
    }

    static registerUser(user) {
        const requestOptions = {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(user)
        };

        return fetch(config.apiUrl + 'API/User/Signup', requestOptions).then(handleResponse, handleError);
    }

    static updateUser(user) {
        const requestOptions = {
            method: 'PUT',
            headers: { ...authHeader(), 'Content-Type': 'application/json' },
            body: JSON.stringify(user)
        };

        return fetch(config.apiUrl + '/users/' + user.id, requestOptions).then(handleResponse, handleError);
    }

    // prefixed function name with underscore because delete is a reserved word in javascript
    static deleteUser(id) {
        const requestOptions = {
            method: 'DELETE',
            headers: authHeader()
        };

        return fetch(config.apiUrl + '/users/' + id, requestOptions).then(handleResponse, handleError);
    }
}

export {
    UserApiProvider
};
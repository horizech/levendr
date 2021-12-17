import { levendrConstants } from '../constants';
import { levendrService } from '../services';
import { alertActions } from './';
import { history } from '../helpers';
import { toastActions } from './toast.actions';

export const levendrActions = {
    checkInitialized,
    initialize
};

function checkInitialized() {
    return dispatch => {
        dispatch(request());

        levendrService.checkInitialized()
            .then(
                result => {
                    if (result.Success) {
                        dispatch(success(result.Message));
                    }
                    else {
                        dispatch(failure(result.Message));
                        dispatch(alertActions.error(result.Message));
                    }
                    localStorage.setItem('levendrStatus', JSON.stringify({
                        checkingStatus: false,
                        isInitialized: result.Success,
                        message: result.Message
                    }));
                },
                err => {
                    dispatch(error(err));
                    dispatch(alertActions.error(err));
                    localStorage.setItem('levendrStatus', JSON.stringify({
                        checkingStatus: false,
                        isInitialized: undefined,
                        message: err
                    }));
                }
            );
    };

    function request() { return { type: levendrConstants.IS_INITIALIZED_REQUEST } }
    function success(message) { return { type: levendrConstants.IS_INITIALIZED_SUCCESS, message } }
    function failure(message) { return { type: levendrConstants.IS_INITIALIZED_FAILURE, message } }
    function error(message) { return { type: levendrConstants.IS_INITIALIZED_ERROR, message } }
}

function initialize(username, email, password) {
    return dispatch => {
        dispatch(request());
        let userInfo={
            "Username": username,
            "Email": email,
            "Password": password
          }
          levendrService.initialize(userInfo)
          .then(
              result => {
                  if(result.Success) {
                      
                      dispatch(success());
                      // history.push(link);
                      dispatch(toastActions.success('Levendr Initilized'));
                      // dispatch(getTableRows(table));
                      return 'Levendr Initilized';    
                  }
                  else {
                      dispatch(failure(result.Message));
                      dispatch(alertActions.error(result.Message));
                      return result.Message;                        
                  }
              },
              error => {
                  dispatch(failure(error.toString()));
                  dispatch(alertActions.error(error.toString()));
              }
          );
        dispatch(success({ Success: true, Message: "Success!" }));
        // dispatch(success({ Success: false, Message: "Error!" }));

    };

    function request() { return { type: levendrConstants.INITIALIZE_REQUEST } }
    function success(message) { return { type: levendrConstants.INITIALIZE_SUCCESS, message } }
    function failure(message) { return { type: levendrConstants.INITIALIZE_FAILURE, message } }
}

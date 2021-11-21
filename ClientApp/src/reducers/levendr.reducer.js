import { levendrConstants } from '../constants';

let isInitialized = JSON.parse(localStorage.getItem('levendrStatus'));
const initialState = isInitialized
    ? isInitialized
    : {
        checkingStatus: false,
        initializing: false,
        isInitialized: undefined,
        message: "Levendr status unknown"
    };

export function levendrReducer(state = initialState, action) {
    switch (action.type) {
        case levendrConstants.IS_INITIALIZED_REQUEST:
            return {
                checkingStatus: true,
                isInitialized: false,
                message: "Checking"
            };
        case levendrConstants.IS_INITIALIZED_SUCCESS:
            return {
                checkingStatus: false,
                isInitialized: true,
                message: action.message
            };
        case levendrConstants.IS_INITIALIZED_FAILURE:
            return {
                checkingStatus: false,
                isInitialized: false,
                message: action.message
            };
        case levendrConstants.IS_INITIALIZED_ERROR:
            return {
                checkingStatus: false,
                isInitialized: undefined,
                message: action.message
            };
        case levendrConstants.INITIALIZE_REQUEST:
            return {
                initializing: true,
                isInitialized: false,
                message: "Checking"
            };
        case levendrConstants.INITIALIZE_SUCCESS:
            return {
                initializing: false,
                isInitialized: true,
                message: action.message
            };
        case levendrConstants.INITIALIZE_FAILURE:
            return {
                initializing: false,
                isInitialized: false,
                message: action.message
            };
        default:
            return state
    }
}
import { combineReducers } from 'redux';

import { levendrReducer } from './levendr.reducer';
import { authenticationReducer } from './authentication.reducer';
import { registrationReducer } from './registration.reducer';
import { usersReducer } from './users.reducer';
import { alertReducer } from './alert.reducer';
import { toastReducer } from './toast.reducer';
import { tablesReducer } from './tables.reducer';
import { currentTableDataReducer } from './current-table-data.reducer';
import { currentTableInfoReducer } from './current-table-info.reducer';
import { predefinedColumnsReducer } from './predefined-columns.reducer';

const rootReducer = combineReducers({
    levendr: levendrReducer,
    authentication: authenticationReducer,
    registration: registrationReducer,
    users: usersReducer,
    alert: alertReducer,
    toast: toastReducer,
    tables: tablesReducer,
    currentTableInfo: currentTableInfoReducer,
    currentTableData: currentTableDataReducer,
    predefinedColumnsInfo: predefinedColumnsReducer
});

export default rootReducer;

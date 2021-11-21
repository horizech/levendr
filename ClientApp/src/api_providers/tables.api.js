import { authHeader, config } from '../helpers';
import { handleError, handleResponse } from './handler.api';


class TablesApiProvider {
    static async getTables() {
        const requestOptions = {
            method: 'get',
            headers: authHeader()
        };

        return fetch(`${config.apiUrl}/api/Table/GetTablesList`, requestOptions)
            .then(handleResponse, handleError)
            .then(result => {
                return result;
            });
    }

    static async getLevendrTables() {
        const requestOptions = {
            method: 'get',
            headers: authHeader()
        };

        return fetch(`${config.apiUrl}/api/Levendr/GetTablesList`, requestOptions)
            .then(handleResponse, handleError)
            .then(result => {
                return result;
            });
    }
    static async getLevendrTableColumns(table) {
        const requestOptions = {
            method: 'get',
            headers: authHeader()
        };

        return fetch(`${config.apiUrl}/api/Levendr/GetTableColumns?table=${table}`, requestOptions)
            .then(handleResponse, handleError)
            .then(result => {
                return result;
            });
    }
    
    static async getTableColumns(table) {
        const requestOptions = {
            method: 'get',
            headers: authHeader()
        };

        return fetch(`${config.apiUrl}/api/Table/GetTableColumns?table=${table}`, requestOptions)
            .then(handleResponse, handleError)
            .then(result => {
                return result;
            });
    }

    static async getPredefinedColumns() {
        const requestOptions = {
            method: 'get',
            headers: authHeader()
        };

        return fetch(`${config.apiUrl}/api/Table/GetPredefinedColumns`, requestOptions)
            .then(handleResponse, handleError)
            .then(result => {
                // console.log(result);
                return result;
            });
    }

    static async insertRows(table,rows) {
        let headers = authHeader();
        headers['Content-Type'] = 'application/json' ;

        const requestOptions = {
            method: 'POST',
            headers: headers,
            body: JSON.stringify(rows)
        };
    
        return fetch(`${config.apiUrl}/api/Table/InsertRows?table=${table}`, requestOptions).then(handleResponse);
    }
    static async getTableRows(table) {
        const requestOptions = {
            method: 'get',
            headers: authHeader()
        };

        // Example of search query
        // const url = `${config.apiUrl}/api/${table}?limit=5&offset=5&orderBy=Id&groupBy=Id`;
        const url = `${config.apiUrl}/api/${table}`;

        return fetch(url, requestOptions)
            .then(handleResponse, handleError)
            .then(result => {
                return result;
            });
    }
    static async updateRows(table, rows) {
        let headers = authHeader();
        headers['Content-Type'] = 'application/json' ;
        
        // console.log(rows);
        const params = {
            
            "Data": rows[0],
            
            "Parameters": [
              {
                "Name": "Id",
                "Condition": 0,
                "Value": rows[0].Id
              }
            ]
          };
        //   console.log(params);
        const requestOptions = {
            method: 'PUT',
            headers: headers,
            body: JSON.stringify(params)
        };
    
        return fetch(`${config.apiUrl}/api/Table/UpdateRows?table=${table}`, requestOptions).then(handleResponse);
    }
    static async deleteRows(table, id) {
        let headers = authHeader();
        headers['Content-Type'] = 'application/json' ;

        const params = [
            {
              "Name": "Id",
              "Condition": 0,
              "Value": id
          
            }
        ];
        console.log(params);
        const requestOptions = {
            method: 'DELETE',
            headers: headers,
            body: JSON.stringify(params)
        };
    
        

        return fetch(`${config.apiUrl}/api/Table/DeleteRows?table=${table}`, requestOptions).then(handleResponse);
    }
    static async createTable(table, ColumnsInfo) {
        let headers = authHeader();
        headers['Content-Type'] = 'application/json' ;

        const requestOptions = {
            method: 'POST',
            headers: headers,
            body: JSON.stringify(ColumnsInfo)
        };
    
        return fetch(`${config.apiUrl}/api/Table/CreateTable?table=${table}`, requestOptions).then(handleResponse);
    }
    static async addColumn(table, ColumnsInfo) {
        let headers = authHeader();
        headers['Content-Type'] = 'application/json' ;

        const requestOptions = {
            method: 'POST',
            headers: headers,
            body: JSON.stringify(ColumnsInfo)
        };
    
        return fetch(`${config.apiUrl}/api/Table/AddColumn?table=${table}`, requestOptions).then(handleResponse);
    }
    static async deleteColumn(table, column) {
        let headers = authHeader();
        headers['Content-Type'] = 'application/json' ;

        const requestOptions = {
            method: 'GET',
            headers: headers
        };
        
        return fetch(`${config.apiUrl}/api/Table/deletecolumn?table=${table}&column=${column}`, requestOptions).then(handleResponse);
    }
   
}
export { TablesApiProvider };

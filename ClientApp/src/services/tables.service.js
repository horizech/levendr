import { TablesApiProvider } from '../api_providers';

export const tablesService = {
    getLevendrTables, getLevendrTableColumns, getTables, getTableColumns, getPredefinedColumns, getTableRows, insertRow, updateRow, deleteRow, createTable, deleteColumn, addColumn
};

function getTables() {
    return TablesApiProvider.getTables();
}

function getTableColumns(table) {
    return TablesApiProvider.getTableColumns(table);
}

function getLevendrTableColumns(table) {
    return TablesApiProvider.getLevendrTableColumns(table);
}

function getPredefinedColumns() {
    return TablesApiProvider.getPredefinedColumns();
}

function getTableRows(table) {
    return TablesApiProvider.getTableRows(table);
}
function insertRow(table, rows) {
    return TablesApiProvider.insertRow(table, rows);
}

function updateRow(table, rows) {
    return TablesApiProvider.updateRow(table, rows);
}

function deleteRow(table, id) {
    return TablesApiProvider.deleteRow(table, id);
}

function createTable(table, ColumnsInfo) {
    return TablesApiProvider.createTable(table, ColumnsInfo);
}

function deleteColumn(table, Column) {
    return TablesApiProvider.deleteColumn(table, Column);
}

function addColumn(table, ColumnsInfo) {
    return TablesApiProvider.addColumn(table, ColumnsInfo);
}
function getLevendrTables(table, ColumnsInfo) {
    return TablesApiProvider.getLevendrTables();
}

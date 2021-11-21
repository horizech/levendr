import { TablesApiProvider } from '../api_providers';

export const tablesService = {
    getLevendrTables, getLevendrTableColumns, getTables, getTableColumns, getPredefinedColumns, getTableRows, insertRows, updateRows, deleteRows, createTable, deleteColumn, addColumn
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
function insertRows(table, rows) {
    return TablesApiProvider.insertRows(table, rows);
}

function updateRows(table, rows) {
    return TablesApiProvider.updateRows(table, rows);
}

function deleteRows(table, id) {
    return TablesApiProvider.deleteRows(table, id);
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

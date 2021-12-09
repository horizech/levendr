import React, { Component } from 'react';
import { connect } from 'react-redux';
import { RiPencilFill, RiDeleteRow } from 'react-icons/ri';
import { tablesActions } from '../actions';
import { CreateEditTableRecordModal } from '../modals'
import { DialogModal } from '../modals'
import { ButtonIcon } from './button-icon.component';
import { Table } from 'reactstrap';
import { Loading } from './loading.component';

import '../styles/LevendrTable.css'

export const LevendrTable = ({headers, children}) => {
    return (
        <div className="levendr-table">
            <Table responsive bordered striped size="sm">
                <thead>
                    <tr key={'levendr_table_header'}>
                        <th key={'levendr_table_header_#'} scope="col"></th>
                        {
                            headers.map(key => (
                                <th key={'levendr_table_header' + key} scope="col">{key}</th>
                            ))
                        }
                    </tr>
                </thead>
                <tbody>{children}</tbody>
            </Table>           
        </div>
    )
}

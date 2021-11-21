import React, { useEffect, useState } from 'react';
import { Button, Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';
// import { CreateEditSettingRecord } from '../components';
import { Form, Field } from 'react-final-form';
import { DynamicElementAdapter } from '../adapters';
import { history, GetElementTypeFromDataType } from '../helpers';

const CreateEditModal = ({ columns, row, label, mode, handleOnClose, selectColumns, selectOptions}) => {
    const [isVisible, setIsVisible] = useState(true);
    const [isJsonVisible, setJsonVisible] = React.useState(true);
    // const [columns, setColumns] = React.useState(null);
console.log(selectOptions);
    const toggle = () => {
        const newIsVisible = !isVisible;
        setIsVisible(newIsVisible);
        if (!newIsVisible) {
            handleOnClose(null);
        }
    }
  
 
    const handleOnSubmit = (values) => {
        const newIsVisible = !isVisible;
        setIsVisible(newIsVisible);
        handleOnClose(values);
    }
    const getIsColumnReadOnly = (column) => {
        switch (column.Name) {
            case "Id": case "CreatedOn": case "CreatedBy": case "LastUpdatedOn": case "LastUpdatedBy": return true;
            default: return false;
        }
    }
    const getIsSelectColumn = (column) => {
        switch (column.Name) {
            case selectColumns[0].Name : case selectColumns[1].Name : return true;
            default: return false;
        }
    }
  
console.log(row);
    return (
        <div>
            <Modal isOpen={isVisible} toggle={toggle} title={label}>
                <ModalHeader toggle={toggle}>{label} Row</ModalHeader>
                <ModalBody>

                    {(columns && columns[0].Name) &&
                        <div>

                            <Form

                                onSubmit={handleOnSubmit}
                                initialValues={{
                                    ...row,
                                }}
                                // validate={handleValidate}
                                render={({ handleSubmit, form, submitting, pristine, values }) => (

                                    <form name="form" className="row" id="createEditSettingForm" onSubmit={(e) => handleOnSubmit(values, e)}>
                                        {columns &&
                                            columns.filter(column => !getIsColumnReadOnly(column) || mode == 'edit').map(column => (
                                                <Field isWorking={false} type="text" key={column.Name} name={column.Name} required="true" component={DynamicElementAdapter} selectOptions={selectOptions[column.Name]} isSelect={getIsSelectColumn(column)} column={column} />
                                            ))
                                        }
                                        {
                                            isJsonVisible && <pre>{JSON.stringify(values, 0, 2)}</pre>
                                        }
                                        <div className="col-md-12">
                                            <button type="button" form="createEditTableForm" value="Submit" className="btn btn-primary m-2"
                                                onClick={() =>
                                                    handleOnSubmit(values)
                                                }
                                            >
                                                {mode === 'create' ? 'Add' : 'Update'}
                                            </button>
                                            <button
                                                type="button" className="btn btn-info m-2"
                                                onClick={form.reset}
                                                disabled={submitting || pristine}
                                            >
                                                Reset
                                            </button>
                                        </div>
                                    </form>

                                )}
                            />
                        </div>
                    }

                </ModalBody>
            </Modal>
        </div>
    );
}

export default CreateEditModal;
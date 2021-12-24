import React from 'react';
import { connect } from 'react-redux';
import { userActions } from '../actions';
import { GetElementTypeFromDataType } from '../helpers';
import { Form, Field } from 'react-final-form';
import { DynamicElementAdapter } from '../adapters';
const RegisterPage = ({ dispatch, checkingStatus, isInitialized, loggedIn, location, history }) => {
    const [submitted, setSubmitted] = React.useState(false);
    const columns = [
        { Name: 'Username', value: 'username', Datatype: 'ShortText' },
        { Name: 'Fullname', value: 'Fullname', Datatype: 'ShortText' },
        { Name: 'Email', value: 'email', Datatype: 'ShortText' },
        { Name: 'Password', value: 'password', Datatype: 'ShortText' }
    ]

    const handleOnSubmit = (values) => {
        // e.preventDefault();
        console.log(values);
        setSubmitted(true);
        // const { dispatch } = props;/
        if (values.Username && values.Email && values.Password) {
            dispatch(userActions.register(values));
        }
    }
    React.useEffect(()=>{
        if (loggedIn && location.pathname === '/login') {
            history.push('/');
            return <p>Going to home...</p>
          }
          
          if(!checkingStatus){
              if(isInitialized === undefined){
                  history.push('/start');
              }
              else if(isInitialized === false){
                  history.push('/initialize');
              }
          }
    });
    return (
        <React.Fragment>

            {(columns && columns[0].Name) &&
                <div>

                    <Form

                        onSubmit={handleOnSubmit}
                        initialValues={{
                           
                        }}
                        // validate={handleValidate}
                        render={({ handleSubmit, form, submitting, pristine, values }) => (

                            <form name="form" className="row" id="createEditSettingForm" onSubmit={(e) => handleOnSubmit(values, e)}>
                                {columns &&
                                    columns.map(column => (
                                        <Field isWorking={false} type={GetElementTypeFromDataType(column.Datatype)} key={column.Name} name={column.Name} component={DynamicElementAdapter} required="true" column={column} />
                                    ))
                                }
                                <div className="col-md-12">
                                    <button type="button" form="createEditTableForm" value="Submit" className="btn btn-primary m-2"
                                        onClick={() =>
                                            handleOnSubmit(values)
                                        }
                                    >
                                        Signup
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

        </React.Fragment>

    );
}
function mapStateToProps(state) {
    const { loggingIn, loggedIn } = state.authentication;
    const { checkingStatus, isInitialized } = state.levendr;
    return {
        checkingStatus,
        isInitialized,
        loggedIn,
        loggingIn
    };
}
const connectedRegister = connect(mapStateToProps)(RegisterPage);
export { connectedRegister as RegisterPage };
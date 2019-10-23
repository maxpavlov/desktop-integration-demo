import React from 'react';
import './App.css';

class AddPatient extends React.Component {
    constructor(props) {
        super(props);
        this.onSubmit = this.onSubmit.bind(this);
        this.state = { inputValue: '', isInputShown: false };
    }

    onSubmit(event) {
        event.preventDefault();
        var newItemValue = this.refs.patientName.value;

        if(newItemValue) {
            this.props.addPatient(newItemValue);
            this.refs.form.reset();
        }
    }

    render () {
        return (
            <form ref="form" onSubmit={this.onSubmit} className="form-inline">
                <input type="text" ref="patientName" className="form-control" placeholder="add a new patient..."/>
                <button type="submit" className="btn btn-default">Add</button>
            </form>
        );
    }
}

class Patients extends React.Component {
    constructor(props) {
        super(props);
        this.onAddPatient = this.onAddPatient.bind(this);
        this.onPatientSelected = this.onPatientSelected.bind(this);
        this.state = {
            patients: props.patients,
            newPatientName: '',
            showAddPatient: false,
            currentPatient: null
        };
    }

    onAddPatient(newPatientName) {
        if(newPatientName) {
            this.props.onNewPatient(newPatientName);
            const state = this.state;
            state.showAddPatient = !state.showAddPatient;
            this.setState(state);
        }
    }

    showHide = () => {
        const state = this.state;
        state.showAddPatient = !state.showAddPatient;
        this.setState(state);
    };

    onPatientSelected(selectedPatientName) {
        const state = this.state;
        state.currentPatient = selectedPatientName;

        this.setState(state);

        this.props.onSelectPatient(selectedPatientName);

    };

    render() {
        let listItems = this.state.patients.map((patient) =>
            <li className={"list-group-item " + (this.state.currentPatient === patient.name && 'active') } key={patient.name} onClick={() => this.onPatientSelected(patient.name)}>
                {patient.name}
            </li>);

        return (<div className="Patients">
            <div className="Upper">
                <h2>Patients</h2>
                <ul className="list-group">{listItems}</ul>
            </div>
            <div className="AddPatient">
                <button type="submit" onClick={this.showHide} className="btn btn-default">
                    { this.state.showAddPatient ? '-' : '+' }</button>
                {this.state.showAddPatient && <AddPatient addPatient={this.onAddPatient}/>}
            </div>
        </div>);
    }
}

function HeartMeasurements() {
    return <h2>Heart measurements</h2>
}

function EyeSightMeasurements() {
    return <h2>Eyesight measurements</h2>
}

class App extends React.Component {
    constructor(props) {
        super(props);
        this.handleNewPatient = this.handleNewPatient.bind(this);
        this.handlePatientSelected = this.handlePatientSelected.bind(this);
        const patients = [];
        patients.push({name: 'Max', cardiograms: [], eyegrams: []});

        this.state = {
            patients: patients,
            currentPatient: null
        };
    }

    handleNewPatient(name) {
        const state = this.state;
        let newPatient = { name: name, cardiograms: [], eyegrams: [] };
        state.patients.push(newPatient);
        this.setState(state);
    }

    handlePatientSelected(key) {
        const state = this.state;
        state.currentPatient = key;
        this.setState(state);
    }

    render() {
        return (
            <div className="App">
                <div className="PatientList">
                    <Patients
                        patients={this.state.patients}
                        onNewPatient={this.handleNewPatient}
                        onSelectPatient={this.handlePatientSelected}
                    />
                </div>
                <div className="HeartMeasurements">
                    <HeartMeasurements/>
                </div>
                <div className="EyeSightMeasurements">
                    <EyeSightMeasurements/>
                </div>
            </div>
        );
    }
}

export default App;

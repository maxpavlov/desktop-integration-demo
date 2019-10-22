import React from 'react';
import './App.css';



class Patients extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            patients: props.patients,
            newPatientName: ''
        };
    }
    render() {
        let listItems = this.state.patients.map((patient) =>
            <li key={patient.name}>
                {patient.name}
            </li>);

        return (<div>
            <h2>Patients</h2>
            <ul>{listItems}</ul>
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
        const patients = [];
        patients.push({name: 'Max', heartgrams: [], eyegrams: []});

        this.state = {
            patients: patients,
            currentPatient: null
        };
    }

    handleNewPatient(name) {
        const patients = this.state.patients;
        let newPatient = { Name: name };
        patients.push(newPatient);
        this.setState({
            patients: patients
        });
    }

    render() {
        return (
            <div className="App">
                <div className="PatientList">
                    <Patients
                        patients={this.state.patients}
                        onNewPatient={(name) => this.handleNewPatient(name)}
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

import React from 'react';
import './App.css';
import uuid from 'uuid';
import { HubConnectionBuilder } from "@aspnet/signalr";

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
        this.deselectAll = this.deselectAll.bind(this);
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

    showHide = e => {
        e.stopPropagation();
        const state = this.state;
        state.showAddPatient = !state.showAddPatient;
        this.setState(state);
    };

    onPatientSelected = (e, selectedPatientName) => {
        e.stopPropagation();
        const state = this.state;
        state.currentPatient = selectedPatientName;
        state.showAddPatient = false;

        this.setState(state);

        this.props.onSelectPatient(selectedPatientName);

    };

    deselectAll() {
        const state = this.state;
        state.currentPatient = null;

        this.setState(state);

        this.props.onSelectPatient(null);
    }

    render() {
        let listItems = this.state.patients.map((patient) =>
            <li className={"list-group-item " + (this.state.currentPatient === patient.name && 'active') } key={patient.name} onClick={(e) => this.onPatientSelected(e, patient.name)}>
                {patient.name}
            </li>);

        return (<div className="Patients" onClick={this.deselectAll}>
            <div className="Upper">
                <h2>Patients</h2>
                <ul className="list-group">{listItems}</ul>
            </div>
            <div className="AddPatient">
                <button type="submit" onClick={(e) => this.showHide(e)} className="btn btn-default">
                    { this.state.showAddPatient ? '-' : '+' }</button>
                {this.state.showAddPatient && <AddPatient addPatient={this.onAddPatient}/>}
            </div>
        </div>);
    }
}

class HeartMeasurements extends React.Component {
    constructor(props) {
        super(props);
        this.onAddMeasurement = this.onAddMeasurement.bind(this);
        this.getStartClass = this.getStartClass.bind(this);
		this.sendToGround = this.sendToGround.bind(this);
		this.resetMeasurement = this.resetMeasurement.bind(this);
        this.state = {
            cardiograms: props.cardiograms,
            hubConnection: null,
            isMeasurementBeingInstantiated: false,
            isMeasurementInProgress: false,
			measurementId: null
        };
    }
    componentDidMount() {
        const connection = new HubConnectionBuilder()
            .withUrl("http://localhost:55344/hub")
            .build();

        connection.on("ReceiveMessage", (id, message) => {
            const msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
            const encodedMsg = id + " says: " + msg;
            console.log(encodedMsg);
			if(msg == 'STARTED' && this.state.isMeasurementBeingInstantiated && this.state.measurementId === id)
			{
				const state = this.state;
				state.isMeasurementBeingInstantiated = false;
				state.isMeasurementInProgress = true;
				this.setState(state);
			}
			
			if(msg == 'STOPPED' && this.state.isMeasurementInProgress && this.state.measurementId === id)
			{
				const state = this.state;
				state.isMeasurementBeingInstantiated = false;
				state.isMeasurementInProgress = false;
				state.measurementId = null;
				this.setState(state);
			}
			
			if(msg.startsWith('DATA:') && this.state.isMeasurementInProgress && this.state.measurementId === id)
			{
				const state = this.state;
				
				let data = msg.substring(5);
				
				var newCardiogramData = data.split(',').map(Number);
				
				let newCardiogram = { id: id, data: newCardiogramData };
				
				state.cardiograms.push(newCardiogram);
				
				state.isMeasurementBeingInstantiated = false;
				state.isMeasurementInProgress = false;
				state.measurementId = null;
				this.setState(state);
			}
			
        });

        this.state.hubConnection = connection;
 
        this.state.hubConnection.start({ withCredentials: false }).catch(err => console.error(err.toString()));
    }
	
	sendToGround = function(action, params) {
		var e = document.createElement('a');
		e.id = 'sendToGround';
		var strParams = "";
		if(typeof params !== 'undefined') {
		  Object.keys(params).forEach(function(key) {
			strParams += strParams !== "" ? '&' : '';
			strParams += key+'='+encodeURI(params[key]);
		  });
		}
		e.href = strParams.length > 0 ? 'companion'+'://'+action+'?'+strParams : 'companion'+'://'+action;
		document.getElementsByTagName('body')[0].appendChild(e);
		e.click();
		e.parentNode.removeChild(e);
	}

    onAddMeasurement() {
        console.log('Starting cardiogram via static launch page redirect...');
        const measurementId = uuid.v4();
        console.log('The id is: ' + measurementId);
        const message = "invoke-cardiograph"
        this.state.hubConnection.invoke("SendMessage", measurementId, message).catch(function (err) {
            return console.error(err.toString());
        });

        const state = this.state;
        state.isMeasurementBeingInstantiated = true;
		state.measurementId = measurementId;
        this.setState(state);
		
		const parameters = {'measurementId': measurementId};
		
		this.sendToGround('cardiograph', parameters);
    }

    getStartClass() {
        const state = this.state;

        if (state.isMeasurementBeingInstantiated) {
            return 'fa fa-spinner fa-spin';
        } else if (state.isMeasurementInProgress) {
            return 'fa fa-spinner fa-pulse';
        }

        return '';
    }
	
	resetMeasurement() {
		const state = this.state;
        state.isMeasurementBeingInstantiated = false;
		state.measurementId = null;
        this.setState(state);
	}

    render() {
        let listItems = this.state.cardiograms.map((cardiogram) =>
            <li className={"list-group-item"} key={cardiogram.id}>
                {cardiogram.data}
            </li>);

        return (<div>
            <div className="Upper">
                <h2>Heart measurements</h2>
                <ul className="list-group">{listItems}</ul>
            </div>
            <div className="AddCardiogram">
                <button type="submit" onClick={this.onAddMeasurement} className={"btn btn-default shadow-none " + this.getStartClass()}>{!this.state.isMeasurementBeingInstantiated && !this.state.isMeasurementInProgress && '+'}</button>
                {this.state.isMeasurementBeingInstantiated && <div className="InstallPrompt"><span className="Tip">No prompt?&nbsp;</span><a href="http://localhost:55344/static/installer.msi" onClick={this.resetMeasurement} target="_blank" className="Tip">Install companion!</a></div>}
				{this.state.isMeasurementInProgress && <div className="InstallPrompt"><span className="Tip">Measurement is in progress...</span></div>}
            </div>
        </div>);
    }
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
        state.currentPatient = state.patients.find(e => e.name === key);
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
                    {this.state.currentPatient && <HeartMeasurements cardiograms={this.state.currentPatient.cardiograms}/>}
                </div>
                <div className="EyeSightMeasurements">
                    {this.state.currentPatient && <EyeSightMeasurements eyegrams={this.state.currentPatient.eyegrams}/>}
                </div>
            </div>
        );
    }
}

export default App;

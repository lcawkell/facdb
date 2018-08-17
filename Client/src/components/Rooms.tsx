import * as React from 'react';
import * as Styles from './Rooms.css';

let interfaceUrl = 'http://localhost:8000';

export interface RoomsProps {
}

export interface RoomsState {
    rooms:IRoom[],
    inputValue: string
}

interface IRoom {
    id: number,
    title: string,
    area: string
}

export default class Rooms extends React.Component<RoomsProps, RoomsState> {
    constructor(props: RoomsProps) {
        super(props);

        this.state = {
            rooms: [],
            inputValue: ''
        }
    }

    componentDidMount(){
        this.getRooms();
    }

    addRoom = (room:IRoom) => {
        fetch(`${interfaceUrl}/api/room`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                Accept: "application/json"
            },
            body: JSON.stringify({title: room.title, area: room.area})
        }).then(response => {
            console.log(response);
        });
    }

    getRooms = () => {
        fetch(`${interfaceUrl}/api/room`).then(response => {
            response.json().then(data => {
                this.setState({rooms:data});
            });
        });
    }

    onInputChange = (caller) => {
        this.setState({inputValue:caller.target.value});
    }

    public render() {
        return (
            <div className={Styles.root}>
                <h1>Here are a list of rooms that may interest you!</h1>
                <input value={this.state.inputValue} onChange={this.onInputChange} /><button onClick={()=>this.addRoom({id: 0, title: this.state.inputValue, area:'0'})}>Add!</button><br/>
                <div>
                    {
                        this.state.rooms.map(room => {
                            let {id, title, area} = room;

                            return <div className={Styles.card} id={`Room-${id}`}>{title}<span className={Styles.actions}>X</span></div>
                        })
                    }
                </div>
            </div>
        );
    }
}

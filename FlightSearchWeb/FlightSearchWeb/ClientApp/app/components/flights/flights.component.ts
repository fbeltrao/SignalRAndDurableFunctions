import { Component, OnInit, Inject } from '@angular/core';
import { HubConnection } from '@aspnet/signalr-client';
import { Http } from '@angular/http';

@Component({
    selector: 'flights',
    templateUrl: './flights.component.html'
})
export class FlightsComponent implements OnInit {
    private hubConnection: HubConnection;
    private searchResult: any = null;
    private broadcastMessages: string[] = [];

    private origin: string = 'Zurich';
    private destination: string = 'Seattle';
    private startDate: Date = new Date();
    private currentSearchId: string = '';
    private currentSearchDate: Date;
    private currentSearchOrigin: string = '';
    private isSearching: boolean = false;


    constructor(private http: Http, @Inject('BASE_URL') private baseUrl: string) {
        
    }

    // https://stackoverflow.com/questions/105034/create-guid-uuid-in-javascript/8809472#8809472
    // Public Domain/MIT
    generateUUID() { 
        var d = new Date().getTime();
        if (typeof performance !== 'undefined' && typeof performance.now === 'function') {
            d += performance.now(); //use high-precision timer if available
        }
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = (d + Math.random() * 16) % 16 | 0;
            d = Math.floor(d / 16);
        return (c === 'x' ? r : (r & 0x3 | 0x8)).toString(16);
        });
    }

    search() {

        var connectionId: string = this.hubConnection['connection'].connectionId;
        this.currentSearchId = this.generateUUID();
        this.currentSearchDate = this.startDate;
        this.currentSearchOrigin = this.origin;
        this.isSearching = true;

        this.searchResult = null;
        this.http.get(this.baseUrl + `api/flightdata/search?searchId=${this.currentSearchId}&origin=${this.origin}&destination=${this.destination}&startDate=${this.startDate}&connectionId=${connectionId}`).subscribe(result => {
            console.log(result);
        },
            error => {
                this.isSearching = false;
                console.error(error)
            }
        );

    }

    ngOnInit() {
        this.hubConnection = new HubConnection('/hub');
        this.hubConnection.on('SearchResult', (data: any) => {
            const received = `Received search result message: ${data}`;
            console.log(received);
            var parsedResponse = JSON.parse(data);
            if (parsedResponse.searchId == this.currentSearchId) {
                this.searchResult = parsedResponse;
                this.isSearching = false;
            }            
            
        });

        this.hubConnection.on('Broadcast', (data: any) => {
            const received = `Received broadcast: ${data}`;
            console.log(received);
            this.broadcastMessages.push(received);
        });

        this.hubConnection.start()
            .then(() => {
                console.log('Hub connection started')
            })
            .catch(err => {
                console.log('Error while establishing connection')
            });
    }
    
}

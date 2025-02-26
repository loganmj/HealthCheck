import { Component } from '@angular/core';
import { ConnectionService, ConnectionServiceOptions } from 'ng-connection-service';
import { map, Observable } from 'rxjs';
import { environment } from '../environments/environment';

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrl: './app.component.scss',
    standalone: false
})

/**
 * The main app class.
 */ 
export class AppComponent {

  // #region Properties

  /**
   * The title of the page
   */ 
  public title = 'Healthcheck Client';

  /**
   * States whether the internet connectivity has been lost.
   */ 
  public isOffline: Observable<boolean>;

  // #endregion

  // #region Constructors

  /**
   * Constructor
   */ 
  public constructor(private connectionService: ConnectionService) {

    // Setup connection service options
    var options: ConnectionServiceOptions = {
      enableHeartbeat: true,
      heartbeatUrl: `${environment.baseUrl}api/heartbeat`,
      heartbeatInterval: 10000
    };

    this.isOffline = this.connectionService.monitor(options)
      .pipe(map(state => !state.hasNetworkConnection || !state.hasInternetAccess));
  }

  // #endregion
}

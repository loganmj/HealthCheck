import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { tap, catchError, of } from 'rxjs';

/**
 * A component used to perform HealthChecks against the server.
 */ 
@Component({
  selector: 'app-health-check',
  templateUrl: './health-check.component.html',
  styleUrl: './health-check.component.css'
})
export class HealthCheckComponent implements OnInit {

  // #region Properties

  /**
   * Custom http result property.
   */
  public result?: Result;

  // #endregion

  // #region Constructors

  /**
   * Constructor
   */
  constructor(private http: HttpClient) { }

  // #endregion

  // #region Private Methods

  /**
   * Lifetime management method.
   * Called on initialization of the component.
   */
  ngOnInit(): void {
    this.http.get<Result>(`${environment.baseUrl}api/health`).pipe(
      tap(result => this.result = result),
      catchError(error => {
        console.error(error);
        return of(undefined);
      })
    ).subscribe();
  }

  // #endregion
}

// #region Private Interfaces

/**
* Describes a single health check result.
*/
interface Check {
  name: string;
  responseTime: number;
  status: string;
  description: string;
}

/**
 *  Describes a summarized result of all checks.
 */
interface Result {
  checks: Check[];
  totalStatus: string;
  totalResponseTime: number;
}

// #endregion

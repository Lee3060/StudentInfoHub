import { Injectable } from '@angular/core';
import { HttpClient, HttpClientModule, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PostService {
  private url = 'https://localhost:7135/api/Student';

  constructor(private client: HttpClient) { }

    getPosts(){
      return this.client.get(this.url);
    }

  // Post method to send data
  createStudent(student: any): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this.client.post(this.url, student, { headers });
  }
  

}



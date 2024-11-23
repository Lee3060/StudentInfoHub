import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { PostService } from './services/post.service';
import { FormsModule } from '@angular/forms';  // Import FormsModule

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, HttpClientModule, CommonModule, FormsModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
  providers: [PostService]
})



export class AppComponent {
  title = 'StuData';
  posts: any;

  // Define the structure for newStudent for Post method
  newStudent: any =
    {
      "id": 0,
      "studentName": "",
      "gender": "",
      "age": 0,
      "isDeleted": false,
      "subjects": [
        {
          /*"id": 0,*/
          "subjectName": ""
          /*"studentId": 0*/
        }
      ]
    }


  constructor(private post: PostService) { }
  // Method to fetch data when button is clicked
  fetchStudentData() //Event
  {
    this.post.getPosts().subscribe(response => {
      this.posts = response;
      console.log(this.posts);
    })

  }

  addStudent()
  {
        // Ensure subjects array is formatted correctly before sending to API
    const newStudentToSend =
    {
          ...this.newStudent,
      subjects: this.newStudent.subjects.map((subject: { subjectName: string }) =>
      ({
            subjectName: subject.subjectName
       }))
    };

    this.post.createStudent(newStudentToSend).subscribe(response => {
      console.log('Student added:', response);
      alert("Student Added Successfully!")
      this.fetchStudentData();  // Refresh the list after adding a student
    });
  }

}

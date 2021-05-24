# TrainTaskerServer 

Server back end to facilitate a Machine Learning training market place.

This is a a React Web app designed allow a 'Mechanical Turk' type market place for AI Machine Learning training.

    The TrainTaskServer is open source and MIT licensed. The source is located at:
    https://github.com/Aphene/TrainTaskerServer

    A live instance of the front end web apps communicating with this server can be found at:
    http://23.91.21.132/train/index.html
    http://23.91.21.132/dash/index.html

    The app was developed using .net C# / SqlSvr
    
Background:
One of the largest expenses in machine learning is training the Machine Learning application. 
Supervised training usually requires humans to manually produce paired input and output data. For example, 
a self driving application requires untold thousands of images.

While preparing a data pair may be simple for a human to do, it has to be repeated thousands of times. 
Hiring a staff of people to do this can quickly become an expensive proposition. 
One method to mitigate these costs is to out-source these tasks as free-lance ‘gigs’.  
The task is broken down to its simplest elements and is bid out to a group of potential contractors.  
The key to implementing this strategy is to make the underlying infrastructure as automated and ‘frictionless’ as possible. 
The easier it is to perform a training task, the lower the cost of the entire training project.

System Architecture

This application part of system with three components:

    Training Server (this piece).  This is a server application hosting the server data and job results database. 
    The Tasker App communicates exclusively with the Training Data Server. Logon and job requests are handled here. 
    The Task Requester App will also exclusively communicates with this server.

    Tasker Web app. This is where the actual training is performed.This is a ‘phone first’ design, 
    though using a desktop works just as well.  The user of this app will be referred to as the ‘Tasker’ from here on.

    Machine Learning Task Requester App.  This is the client piece for the Machine Learning Task Requester.  
    This is for the developer who wants training data for his ML application and is willing to pay to have it created. 
    The user of this app will be referred to as the ‘Requester’ from here on.
    

This server communicates with the two Web apps via HTTP GET and POST requests.  GET data is passed as URL key/value parameters.
Post data is passed as either as a Json object or as 64 Bit encoded image data.



Data object descriptions

Job
Describes a collection of tasks required to train a Machine Learning app. 
The requester creates a single Job, and uploads multiple images for that Job.

Task
One individual task (one image that needs to be highlighted).  
This task object describes the resources required to perform the task. 
Multiple taskers (tasker web app users) may perform the same task.

Train
This object contains the result of a tasker performing a particular task. 
i.e. highlight coordinates: x, y, width, height.
    


Database Tables

Job Record
ID
userID
name
title
description
instructions
bounty
budget
budgetLeft
active
startTime
endTime
guid
type

TaskRecord
ID
RequesterID
TaskerID
TaskID
JobID
TaskType
ResourcePath
TimeStamp
Result
MaxTrains
TrainCount
guid



Train Record
ID
X
Y
W
H
Result
ResourcePath
TaskRecordID
timeStamp
UserID
RequesterID
JobID
guid


UserRecord
ID
userID
screenName
email
password
balance
isRequester
guid







    
    

    

### Simple Background Task Queue

* Task Queue will process tasks one by one.
  * Mediator also processes tasks one by one upon publishing.
* No task timeout, channel will not move forward until the task is finished.
  * It is possible that the queue will not move forward if task is deadlocked.
  
     i.e., SQL query / stored proc that is deadlock, 3rd party api not responding.
* Controllers / Handlers will need to inject ITaskQueue in order to send notification. 
  * Testing might be complicated since its tightly coupled with the ITaskQueue.

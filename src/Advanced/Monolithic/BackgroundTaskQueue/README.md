### Advanced Background Task Queue

* Task Queue will process tasks ~~one by one.~~
    * Implemented processing tasks by batch, which can also be configurable.
    * ~~Mediator also processes tasks one by one upon publishing.~~
    * Created a Custom Mediator and overriden the PublishCore to process tasks concurrently.
* ~~No task timeout, channel will not move forward until the task is finished.~~
    * ~~It is possible that the queue will not move forward if task is deadlocked.
      i.e., SQL query / stored proc that is deadlock, 3rd party api not responding.~~
* Implemented task timeout through cancellation token source. It will raise OperationCanceledException once timeout
  duration has elapsed, Hence, queue will move forward, avoiding deadlock.
* ~~Controllers / Handlers will need to inject ITaskQueue in order to send notification.~~
  ~~* Testing might be complicated since its tightly coupled with the ITaskQueue.~~
* Created a Dispatch delegate and included in dependency injection, Task Queue and Mediator will be handled outside the
  Handlers, hence the decoupling.
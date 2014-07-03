StreamingWebApi
===============

This was created in conjunction with the blog post: [Streaming data using Web API](http://dblv.github.io/2014/07/02/streaming-web-api/)

The samples are as follows:

StreamingService
----------------

This provides a set of resources powered by WebAPI that allow the download and upload of staff member data and images

StreamingConsumerClient
-----------------------

This is a tiny Angular app that uses the XmlHttpRequest object to stream down a list of staff members from a resource exposed by the service above, and process each chunked response as it arrives. It also uses a separate resource to pull down the image for the staff member.

StreamingConsumerConsole
------------------------

A little console app that streams a replacement image up to the service using HttpClient.


Images were sourced from www.stockvault.net

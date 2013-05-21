#Contrive

Contrive is a collection of utilities for Microsoft .NET based on my experience in many codebases.

Contrive contains an authentication and authorization system.

Most of these tools are in use in production applications, but have not been fully cleaned up and made general purpose for public consumption.

In order to run the sample web application, you will need to have the Adventure Works Lite database running. You can download the database [here](http://msftdbprodsamples.codeplex.com/releases/view/93587).

Also, you will need to run the "spec" When\_updating\_all\_users in the integration folder. This will update the password hashes to use the current machine key.

For the purposes of the sample, the username is the first and last name of the user concatenated. All passwords are: pass@word1

Contrive was influenced by and borrows liberally from:

* [CodeCampServer] (http://codecampserver.codeplex.com/)
* [OpenFaq] (http://openfaq.codeplex.com/)
* [Security-Guard] (https://github.com/kahanu/Security-Guard)
* [Caliburn Micro] (http://caliburnmicro.codeplex.com)
* [MVC Music Store] (http://mvcmusicstore.codeplex.com/)

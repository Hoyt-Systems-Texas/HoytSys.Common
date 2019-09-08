# System/Application Checklist

- [ ] What data does the application own?
- [ ] Does the application need to be able to push events to a user or system?
- [ ] How important is the performance of the application?
    - [ ] If important define the expected performance.
- [ ] Are there any regulations affecting the application?
    - [ ] What regulations affect the application?
    - [ ] What are the auditing requirements?
- [ ] Define the high-level design.
- [ ] How is the application going to be secured?
    - [ ] What is the maximum session length?
    - [ ] How long are biometric security valid for?
    - [ ] Session and long term keys are able to be expired.
    - [ ] Security is upfront and not possible to ignore in the backend.  Means you have to explicitly remove or ignore security roles.
- [ ] What is the financial risk of a security issue or a data leak?
- [ ] Define a glossary of terms with the business and development team.
- [ ] Define high-level domain model.
- [ ] Sit with the users and understand the current high-level process and pain points.  Document the process and sit with the user to verify it's mostly correct.  Observe first to see if it's correct then show it to the users.
- [ ] What are the availability requirements?
    - [ ] Does the system need to be clustered?
- [ ] What features does the system have to have to get to a Pilot quickly?
- [ ] Do we need to be able to cache data?
    - [ ] How bad is it to show stale data?
    - [ ] How are the caches expired?
    - [ ] How long should the data be cached for?
    - [ ] How does the data get refreshed?

# Feature Checklist

- [ ] Define the current workflow.
- [ ] How complex is the feature vs business value added?
- [ ] Can this be automated?
- [ ] Can the workflow be changed to make it easier to code?
- [ ] Can the workflow be changed to make it harder to make a mistake?
- [ ] What are the business risks having this feature?
- [ ] What are the business risks not having this feature?
- [ ] Does the feature affect security? If it does how is the risk mitigated?
- [ ] What is the needed performance for this feature?
- [ ] What are the potential edge cases that could come up?
- [ ] How often will the feature be used?

# Development Checklist

- [ ] Defined a model on the expected behavior.  If simple feature/library just mental model otherwise need basic documentation.
- [ ] Does the current development have the expected behavior?  If not does it still useful or is it getting unexpectedly complex?
- [ ] What are the potential issues?
- [ ] What are the plans if the potential series issues occur?
- [ ] Who will uses the code?  IE does it potentially have external application uses or just used internally for the application.
- [ ] What is the expected reliability of the code?
- [ ] What will happen if the backing data source goes away?  If something bad could happen how do we recover?
- [ ] Does the complexity out way the usefulness?
- [ ] Is the design flexible enough to handle any unknown edge cases?
- [ ] All data render has been escaped unless already done.
- [ ] Any potential security issues.
- [ ] Any cookies are have Set-Cookie HttpOnly, Secure.

# Deployment Checklist

- [ ] Have the correct security headers been applied?
- [ ] Make sure the application forces secure connection.
- [ ] Do not allow CORS globally in a production environment.
- [ ] All the protected urls return unauthorized access if not an active session.
- [ ] All numbers used for security use a secure random number generator and are at least 128 bits long.
- [ ] Secure cookies have been used for storing the session information.
- [ ] User is not trusted to provide security information like user id and roles unless it has been signed.
 - [ ] User access is always validate in the code.
 - [ ] TLS 1.0 and lower have access disabled.
- [ ] Set the most restrictive headers from: https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers.

# Possible Ways to Secure A System

* Generating secure random numbers for exposed keys.
* Signed urls which a shared secret or a certificate.
* Always use the user who is requesting the data access level.
* Separate the backend processes from user level process.
* Always use secure cookies.

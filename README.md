# Signature Lookup

This is a very simple application that allows you to lookup the signature of a any user in your own tenant.


## Usage

You can run it locally be running `docker run -p 5000:5000 gaardsholt/signature-lookup` and then you can access it at http://localhost:5000/{e-mail}/{type}, fx. http://localhost:5000/firstname.lastname@domain.local/html.

The `{type}` part of the url is optional but defaults to `html`.<br>
Valid options are `html` or `text`.


## Configuration

For this application to work, you need to provide it with credentials to a service account that has access to read mailbox settings in your Exchange Online tenant.

You can provide these credentials in one of two ways, [environment variable](#Environment-variables) or a [json file](#JSON-file).

<br>

### Environment variables

You need to set two environment variables, `Credentials__Username` for the Username and `Credentials__Password` for the Password of your service account.


### JSON file

Set the environment variable `EXTRA_CONFIG` as the path to a JSON file containing the following:
```json
{
  "Credentials": {
    "Username": "service.account@domain.local",
    "Password": "password-for-set-service-account"
  }
}
```

<br><br><br><br>
## Todo:
* Trim docker images size
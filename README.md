# Vault-Config
This app is designed to configure vault settings via the vault rest api and using the C# vaultsharp SDK. All of the settings are confgured via a yaml file that is passed in via env arg to the container.

The purpose of this is for an easy way of continuous delivery for docker or Kubernetes Hashicorp vault deployments.

NOTE: This requires to store the root and unseal tokens in a file in the container mounted on a volume. This is most likely not the best approach for a production environment. Please make note of that and only use this for homelab/dev environments.

How to include links

## Inpiration
inpired by the vault-operator made by Banzaicloud used for kubernetes. This app initializes and configures vault based on a yaml file. Wanted something that would work for Docker. 
Link: https://bank-vaults.dev/docs/operator/

I was also inpired by Khuedoans homelab for the dynamic secrets defined in a yaml file to be created in vault.
Link: https://github.com/khuedoan/homelab

The vault-init and storing the unseal key + root token was inpired by this project: https://github.com/with-shrey/VaultDevSetup-Docker/tree/main

## How to use
There is an example vault docker compose file in the examples folder. All you will need to do is replace the uri env var with the uri of the vault server.

### Secrets
To keep secrets such as client secrets for oidc config this app supports replacing env vars that are contained in the yaml. To do this put the name of the env var under the envs section in the yaml config, pass in an env var with the same name and the value into the container.

When the program starts it will do a string replace for any occurences of the env var name with the env var value passed into the container. Make sure that the name of the env var does not match any value that you do not want to be changed.

### Examples
In the examples folder there is an example docker compose file that should load up both a vault instance and an instance of this project.

## Usage
Currently only tested using Docker Compose. Should work for Kubernetes but have not tested it.

## Contribute
If there any bugs or a feature is missing feel free to put in an issue or a PR.



## Roadmap:
- Better comparison between items in vault vs config file.
- Kubernetes file/Helm chart
- Create individual user folders by using keycloak sdk?
- Look into PR for Vault Sharp for the OIDC, Groups and Group Alias methods.

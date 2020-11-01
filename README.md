# Strong531

This project generates training plans for multiple users based on [Jim Wendler](https://jimwendler.com)'s excellent [5-3-1 method](https://jimwendler.com/blogs/jimwendler-com/101065094-5-3-1-for-a-beginner).

It reads a configuration file located in `~/.Strong531/config.yaml` that contains the 1RM of each user. The number of cycles in the plan can be passed as a command-line argument (2 by default)

It generates a plan for each user and prints it to the console in YAML format.

Here is what the config file looks like:

```
Guy:
   Press: 110
   Deadlift: 200
   Bench: 155
   Squat: 165
Liat:
    Press: 100
    Deadlift: 225
    Bench: 135
    Squat: 165
Gigi:
    Press: 115
    Deadlift: 275
    Bench: 195
Saar:
    Press: 145
    Deadlift: 275
    Bench: 225
    Squat: 165
```    

# Projects

There are 3 C# projects:

- Strong531
- Strong531Tests
- Strong531ConsoleApp

Strong531 is a class library that does all the heavy lifting.

Strong531Tests contains unit tests for String531

Strong531ConsoleApp is a console application that reads the configuration file, gets the number of cycles from the command-line arguments, invokes Strong531 and finally prints out the generated plan.

There is also a Python program called `google_sheet_manager.py` that takes the generated and populates each cycle into a Google sheet.

# Installation instructions for google_sheet_manager

- Install [pyenv](https://github.com/pyenv/pyenv) or [pyenv-win](https://github.com/pyenv-win/pyenv-win)
- Install [poetry](https://python-poetry.org/docs/#installation)

Create a Python 3.9 environment

```
$ pyenv install 3.9.0
$ pyenv local 3.8.9
$ poetry init
$ poetry env use 3.9.0
$ poetry install
```

# Google sheet access and authentication

Follow the instructions here for service account authentication:

https://gspread.readthedocs.io/en/latest/oauth2.html

You should end up with a file called service_Account.json in `~/.config/gspread/` 

# Usage

First generate a plan by running Strong531ConsoleApp.

Then populate a Google spreadsheet by running google_sheet_manager.py
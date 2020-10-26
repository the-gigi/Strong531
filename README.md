# Strong531

This project generates training programs based on [Jim Wendler](https://jimwendler.com)'s excellent [5-3-1 method](https://jimwendler.com/blogs/jimwendler-com/101065094-5-3-1-for-a-beginner).

It reads a configuration file located in `~/.Strong531/config.yaml` that contains the 1RM of each user.
The number of cycles in the plan can be passed as a command-line argument (2 by default)

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


# greenergy
The idea is to develop something to make it easy to climate-optimize electricity consumption.
The initial "plan" is to:
1. Fetch co2/kwh predictions from the Danish energinet (https://energinet.dk/).
2. Do some simple experiments with exposing the data through a chatbot ("when should I charge my car tonight" etc.)
3. Define a set of IFTTT events to allow IFTTT actions to be kicked of when co2/kwh is low or high, and to predict when this may occur over the coming X hours.

Longer term, who knows.

Technology wise, this will be a containerized application configured to run in Kubernetes.
Most coding will happen in .net core and C#, using Visual Studio Code.

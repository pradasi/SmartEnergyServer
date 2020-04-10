import requests
import json

urlForWeather =  "https://api.weather.com/v3/wx/forecast/daily/15day?apiKey=6532d6454b8aa370768e63d6ba5a832e&geocode=13.20%2C77.71&language=en-US&units=e&format=json"

try:
    request = requests.get(url = urlForWeather)
    data = request.json()
    weatherDataForWind = {}
    
    dateTick = data['validTimeLocal']
    day = data['dayOfWeek']
    temperatureMax = data['temperatureMax']
    temperatureMin = data['temperatureMin']
    narrative = data['narrative']
    weatherDataForWeek = []
    for i in range(len(dateTick)):
        temp = {}
        date = dateTick[i].split('T')[0]
        temp['date'] = date 
        temp['day'] = day[i]
        temp['maxTemperature'] = str(temperatureMax[i]) + " F"
        temp['minTemperature'] = str(temperatureMin[i]) + " F"
        temp['weatherCondition'] = narrative[i].split(".")[0]
        weatherDataForWeek.append(temp)
    
    myJson = json.dumps(weatherDataForWeek)
    print(myJson)

except Exception as ex:
	print("Failed, the exception is {}".format(ex))
	
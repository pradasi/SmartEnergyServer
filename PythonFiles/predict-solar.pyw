#to load the model
from tensorflow.keras.models import load_model
#ppackage for getting angles and radiation
import meteomatics.api as api
#for datetime generation
import datetime as dt
#to convert to dataframe
from pandas import DataFrame
#fro scaling
from sklearn.preprocessing import MinMaxScaler
#to join two list
from numpy import concatenate

#getting current date and preperation to call api
dateTime = dt.datetime.utcnow().strftime("%Y-%m-%dT%H:%M:%S+00:00")
url = "https://api.meteomatics.com/"+dateTime+"P2D:PT1H/sun_elevation:d,sun_azimuth:d,t_2m:C,relative_humidity_1000hPa:p,sfc_pressure:hPa,global_rad:W/12.97210,77.59330/json"
username = 'cmrit_kumar'
password = 'aM3OVy9uYz7Ei'


#function to extract value from json
def extractValue(data):
    valueList = []
    for i in range(len(data)):
        valueList.append(data[i]['value'])
    return valueList


try:
    #calling the api
    data = api.query_api(url, username, password )
    #on succesfull call data extraction process
    featuresToPredict = data.json()['data']
    
    toPredict = []    
    toPredict.append(extractValue(featuresToPredict[0]['coordinates'][0]['dates'][1:25]))    
    toPredict.append(extractValue(featuresToPredict[1]['coordinates'][0]['dates'][1:25]))    
    toPredict.append(extractValue(featuresToPredict[2]['coordinates'][0]['dates'][1:25]))    
    toPredict.append(extractValue(featuresToPredict[3]['coordinates'][0]['dates'][1:25]))   
    toPredict.append(extractValue(featuresToPredict[4]['coordinates'][0]['dates'][1:25]))  
    toPredict.append(extractValue(featuresToPredict[5]['coordinates'][0]['dates'][0:24]))
	##dummy
    solarEnergy = extractValue(featuresToPredict[5]['coordinates'][0]['dates'][1:25])
    
    #preparing to scale
    scaler = MinMaxScaler(feature_range=(0,1))
    
    featuresToPredict = DataFrame(toPredict).T
    
    featuresToPredict = featuresToPredict.astype('float32')
    
    scaled = scaler.fit_transform(featuresToPredict)
    
    #reshaping for prediction
    values = scaled.reshape(scaled.shape[0], 1 , scaled.shape[1])
    
    #load the model
    model = load_model('PythonFiles\my_solar_model')
    
    #predicting
    yhat = model.predict(values)
    
    #converting back to normal shape
    values = values.reshape((values.shape[0], values.shape[2]))
    
    #inversing the scaling
    inv_yhat = concatenate((values[:, :-1], yhat), axis=1)
    inv_yhat = scaler.inverse_transform(inv_yhat)
    inv_yhat = inv_yhat[:,-1]
    
    #initilizing dictionary
    preditctedSolarData = {}
    
    #generate dateTime range
    now= dt.datetime.now()
    date_list = [now + dt.timedelta(minutes=60*x) for x in range(0, 25)]
    indexDate=[x.strftime("%H:%M") for x in date_list]
    indexDate = indexDate[1:]
	#golmal for now
    #enumerating and appending
    for i,date in enumerate(indexDate):
        preditctedSolarData[date] = solarEnergy[i]
    
    import json
    
    myJson = json.dumps(preditctedSolarData)
    print(myJson)
    
except Exception as ex:
    print("Failed, the exception is {}".format(ex))

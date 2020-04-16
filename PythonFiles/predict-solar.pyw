import os
os.environ['TF_CPP_MIN_LOG_LEVEL'] = '3'

#to load the model
from tensorflow.keras.models import load_model
#csv reading
from pandas import read_csv
#for scaling
from sklearn.preprocessing import MinMaxScaler
#to join two list
from numpy import concatenate
#to concat two numpy array
from numpy import column_stack
#to convert to json
import json
#for datetime
import datetime as dt
#to disable debug

try:
	
	data = read_csv('PythonFiles\weatherForecastForSolar.csv', header=0)
	
	#separarting the data
	featuresUsedToPredict = data.values[:,:-2]
	prevSolarData = data.values[:,-2]
	solarEnergy = data.values[:,-1]
	
	#preparing to scale
	scaler = MinMaxScaler(feature_range=(0,1))

	#combining the data according to our model
	featuresUsedToPredict = column_stack((featuresUsedToPredict,prevSolarData))
	
	#ensure all data is float
	featuresToPredict = featuresUsedToPredict.astype('float32')

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

	#extracting the index dates for json
	indexDate = data.index.to_list()
	
	#initilizing dictionary
	preditctedSolarData = {}

	#for date
	dateHour = []
	now= dt.datetime.now()
	date_list = [now + dt.timedelta(minutes=60*x) for x in range(0, 24)]
	fullHour = (now + dt.timedelta(minutes=60*1)).strftime("%H:%M")
	indexDate =[x.strftime("%H") for x in date_list]

	dateHour = indexDate[1:]

	preditctedSolarData['FullHour'] = fullHour
	preditctedSolarData['Hour'] = dateHour
	preditctedSolarData['value'] = solarEnergy.tolist()
	
	myJson = json.dumps(preditctedSolarData)
	print(myJson)
    
except Exception as ex:
    print("Failed, the exception is {}".format(ex))

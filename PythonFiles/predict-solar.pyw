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

try:
	
	data = read_csv('PythonFiles\weatherForecastForSolar.csv', header=0, index_col=0)
	
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
	#enumerating and appending
	for i,date in enumerate(indexDate):
		dateHour.append(date.split(":")[0])

	preditctedSolarData['FullHour'] = indexDate[0]
	preditctedSolarData['Hour'] = dateHour
	preditctedSolarData['value'] = solarEnergy.tolist()
	
	myJson = json.dumps(preditctedSolarData)
	print(myJson)
    
except Exception as ex:
    print("Failed, the exception is {}".format(ex))

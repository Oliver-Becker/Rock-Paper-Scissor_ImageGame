import sys
import pickle
import imageio
import numpy as np
import skimage.morphology as morph
from skimage.transform import resize
from matplotlib.colors import rgb_to_hsv
from skimage.feature import local_binary_pattern, hog
from warnings import filterwarnings

filterwarnings('ignore')

def median_filter(img, filterSize=3):
  filterRadius = filterSize//2
  img_pad = np.pad(img, ((filterRadius, ),(filterRadius, )), 'edge')


  filtered_img = np.zeros(img_pad.shape)
  for x in np.arange(filterRadius, img_pad.shape[0]-filterRadius+1):
    for y in np.arange(filterRadius, img_pad.shape[1]-filterRadius+1):
      med_region = np.median(img_pad[x-filterRadius:x+filterRadius+1, y-filterRadius:y+filterRadius+1])
      filtered_img[x,y] = med_region

  return filtered_img[filterRadius:img_pad.shape[0]-filterRadius, filterRadius:img_pad.shape[1]-filterRadius]

def normalize(img):
    img_new = (((img - np.amin(img))*255)/(np.amax(img) - np.amin(img))).astype(np.uint8)
    return img_new

def preprocessing(img):
  aux = resize(img, (150, 150, 3), anti_aliasing=False)
  aux = rgb_to_hsv(aux)
  aux = normalize(aux[:,:,2])
  aux = median_filter(aux)
  aux = aux - morph.erosion(aux, morph.disk(2))

  return aux

def classify(img):
  #Preprocessing the image to extract features
  img_processed = preprocessing(img)

  aux = local_binary_pattern(img_processed, 8, 1)
  hist, bins = np.histogram(aux, int(aux.max() + 1), density=True)

  hog_v = hog(img_processed, feature_vector=True)
  features = np.append(hist,hog_v)

  loaded_model = pickle.load(open('finalized_model.sav', 'rb'))

  y = loaded_model.predict(features.reshape(1, -1))

  print(y[0])
  return y[0]

if len(sys.argv) == 2 :
  filePath = sys.argv[1]
  img = imageio.imread(filePath)
  classify(img)
else :
  print("Error! Argc != 2. You need to pass exactly 1 argument to this script.")
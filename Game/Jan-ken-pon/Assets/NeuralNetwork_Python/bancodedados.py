# -*- coding: utf-8 -*-
"""BancoDeDados.ipynb

Automatically generated by Colaboratory.

Original file is located at
    https://colab.research.google.com/drive/1cFSP1Xdr9bdITrDNx48q5GeoTr5FadNa
"""

import os
import imageio
import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
from skimage.transform import resize
from matplotlib.colors import rgb_to_hsv
from skimage.transform import rotate, AffineTransform
from skimage import transform
from google.colab import drive
drive.mount('/content/drive')

paper_path = "/content/drive/Shared drives/Equipe amigos/2020.1/PDI/fotos/paper"
rock_path = "/content/drive/Shared drives/Equipe amigos/2020.1/PDI/fotos/rock"
scissor_path = "/content/drive/Shared drives/Equipe amigos/2020.1/PDI/fotos/scissor"
paper = []
scissor = []
rock = []


for filename in os.listdir(paper_path): 
  paper.append(imageio.imread(paper_path + '/' + filename))

for filename in os.listdir(scissor_path): 
  scissor.append(imageio.imread(scissor_path + '/' + filename))

for filename in os.listdir(rock_path): 
  rock.append(imageio.imread(rock_path + '/' + filename))

for i in range(len(paper)):
  paper[i] = resize(paper[i], (150, 150, 3), anti_aliasing=False)
for i in range(len(scissor)):
  scissor[i] = resize(scissor[i], (150, 150, 3), anti_aliasing=False)
for i in range(len(rock)):
  rock[i] = resize(rock[i], (150, 150, 3), anti_aliasing=False)

def normalize(img):
    img_new = (((img - np.amin(img))*255)/(np.amax(img) - np.amin(img))).astype(np.uint8)
    return img_new

for i in range(len(paper)):
  paper[i] = rgb_to_hsv(paper[i])
  paper[i] = normalize(paper[i][:,:,2])
for i in range(len(scissor)):
  scissor[i] = rgb_to_hsv(scissor[i])
  scissor[i] =  normalize(scissor[i][:,:,2])
for i in range(len(rock)):
  rock[i] = rgb_to_hsv(rock[i])
  rock[i] =  normalize(rock[i][:,:,2])

def augmentation(img, val):
  
  rotate30 = np.append(np.reshape(normalize(rotate(img, angle=30, cval=1)), (img.size)), val)
  rotate45 = np.append(np.reshape(normalize(rotate(img, angle=45, cval=1)), (img.size)), val)
  rotate60 = np.append(np.reshape(normalize(rotate(img, angle=60, cval=1)), (img.size)), val)
  rotate90 = np.append(np.reshape(normalize(rotate(img, angle=90, cval=1)), (img.size)), val)
  up_down = np.append(np.reshape(np.flipud(img), (img.size)), val)
  left_right = np.append(np.reshape(np.fliplr(img), (img.size)), val) 


  return np.vstack((rotate30, rotate45, rotate60, rotate90, up_down, left_right))

paper_concat = np.append(np.reshape(paper[0], (paper[0].size)),1)
scissor_concat = np.append(np.reshape(scissor[0], (scissor[0].size)),2)
rock_concat = np.append(np.reshape(rock[0], (rock[0].size)),3)

for i in range(1, len(paper)):
    paper_concat = np.vstack((paper_concat, np.append(np.reshape(paper[i], (paper[i].size)),1), augmentation(paper[i], 1)))

for i in range(len(scissor)):
    scissor_concat= np.vstack((scissor_concat, np.append(np.reshape(scissor[i], (scissor[i].size)),2), augmentation(scissor[i], 2)))

for i in range(len(rock)):
    rock_concat = np.vstack((rock_concat, np.append(np.reshape(rock[i], (rock[i].size)),3), augmentation(rock[i], 3)))
    
paper_concat = np.vstack((paper_concat, scissor_concat, rock_concat))

print(paper_concat)

pd.DataFrame(data=paper_concat).to_csv('/content/drive/Shared drives/Equipe amigos/2020.1/PDI/bancodedados.csv', index=False, index_label=False)
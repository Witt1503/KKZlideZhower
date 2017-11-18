#!/usr/bin/env python
#
#  Copyright (c) 2013, 2015, Corey Goldberg
#
#  Dev: https://github.com/cgoldberg/py-slideshow
#  License: GPLv3


import argparse
import random
import os

import pyglet

def defineGlobals():
	global _pan_speed_x
	global _pan_speed_y
	global _zoom_speed
	global _ad_frequency
	global _ad_count
	global _show_time
	_pan_speed_x = 0
	_pan_speed_y = 0
	_zoom_speed = 0
	_ad_frequency = 3
	_ad_count = 1
	_show_time = 1
	



def update_pan(dt):
	sprite.x += dt * _pan_speed_x
	sprite.y += dt * _pan_speed_y


def update_zoom(dt):
	sprite.scale += dt * _zoom_speed


def update_image(dt):
	global _ad_frequency
	global _ad_count
	global _show_time
	#print('Time since last: ' + str(dt) + '\n ' + str(_ad_count))
	if (_ad_count % _ad_frequency)==0:
		#print('Show time: '+str(2*_show_time))
		pyglet.clock.schedule_once(update_image, 2*_show_time)
		img = pyglet.image.load(random.choice(ad_paths))
		_ad_count = 1
		label.text = ''
	else:
		pyglet.clock.schedule_once(update_image, _show_time)
		path = random.choice(image_paths)
		img = pyglet.image.load(path)
		_ad_count = _ad_count +1
		split = path.split('\\');
		label.text = split[len(split)-2]
	sprite.image = img
	sprite.scale = get_scale(window, img)
	sprite.x = 0
	sprite.y = 0
	window.clear()


def get_image_paths(input_dir='./images/'):
	paths = []
	#print(input_dir)
	for root, dirs, files in os.walk(input_dir):
		for file in sorted(files):
			if file.endswith(('jpg', 'png', 'gif','jpeg')):
				path = os.path.abspath(os.path.join(root, file))
				split = path.split('\\');
				if split[len(split)-2] == 'Reklamer':
					continue
				paths.append(path)
				#print(split[len(split)-2])
	return paths
	
def get_ad_paths(input_dir='./images/'):
	paths = []
	for root,dirs, files in os.walk(input_dir+'/Reklamer'):
		#print(files)
		for file in sorted(files):
			if file.endswith(('jpg', 'png', 'gif','jpeg')):
				path = os.path.abspath(os.path.join(root, file))
				paths.append(path)
				#print(path)
	return paths


def get_scale(window, image):
	if image.width > image.height:
		scale = float(window.width) / image.width
	else:
		scale = float(window.height) / image.height
	return scale


window = pyglet.window.Window(fullscreen=True)


@window.event
def on_draw():
	sprite.draw()
	label.draw()


if __name__ == '__main__':
	defineGlobals()
	parser = argparse.ArgumentParser()
	parser.add_argument('dir', help='directory of images',
						nargs='?', default=os.getcwd())
	args = parser.parse_args()

	image_paths = get_image_paths(args.dir)
	ad_paths = get_ad_paths(args.dir)
	img = pyglet.image.load(random.choice(image_paths))
	sprite = pyglet.sprite.Sprite(img)
	sprite.scale = get_scale(window, img)
	label = pyglet.text.Label('',
                          font_name='Times New Roman',
                          font_size=36,
                          x=window.width//2, y=36,
                          anchor_x='center', anchor_y='center')
	

	pyglet.clock.schedule_once(update_image, _show_time)

	pyglet.app.run()

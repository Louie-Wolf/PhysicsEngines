# -*- coding: utf-8 -*-
"""
Created on Sun Feb 18 22:39:00 2024

@author: wolflou1, zwahlni2
"""
import numpy as np
import matplotlib.pyplot as plt
import csv

# Time Intervalls for Graphs: -------------------------------------------------

t_from_normal = 6
t_to_normal = 18

t_from_zoom = 12.3
t_to_zoom = 13.5

t_from_rot = 10
t_to_rot = 40

# -----------------------------------------------------------------------------
t2 = []
x2_t = []
z2_t = []
v2_t = []
p2_t = []
eKin2_t = []
# Grössen zur Rotation des L Förmigen Körpers mit blauem Würfel attached:
alpha_t = []
vRot_t = []
pIntrinsicRot_t = []
pOrbitalRot_t = []
pRotTot_t = []
eRot_t = []
# Grössen zur Translation des Schwerpunkts von L Förmigen Körper + blauer Würfel
xTrans_t = []
zTrans_t = []
vTrans_t = []
pTrans_t = []
eTrans_t = []

eSpr_t = []


with open('stationaryCube_stats.csv', 'r') as csvfile2:
    # Überspringe die Header-Zeile
    next(csvfile2)
    plots = csv.reader(csvfile2, delimiter = ',')
    
    for row in plots:
        t2.append(float(row[0]))
        x2_t.append(float(row[1]))
        z2_t.append(float(row[2]))
        v2_t.append(float(row[3]))
        p2_t.append(float(row[4]))
        eKin2_t.append(float(row[5]))
        alpha_t.append(float(row[6]))
        vRot_t.append(float(row[7]))
        pIntrinsicRot_t.append(float(row[8]))
        pOrbitalRot_t.append(float(row[9]))
        pRotTot_t.append(float(row[10]))
        eRot_t.append(float(row[11]))
        xTrans_t.append(float(row[12]))
        zTrans_t.append(float(row[13]))
        vTrans_t.append(float(row[14]))
        pTrans_t.append(float(row[15]))
        eTrans_t.append(float(row[16]))


plt.figure(6)
plt.plot(t2, z2_t, color='b', linestyle='-', label= 'blue Cube')
plt.plot(t2, zTrans_t, color='y', linestyle='-', label= 'Center of Mass')
plt.xlabel("Time [s]")
plt.ylabel("z-Position [m]")
plt.title("z-Position over Time")
plt.xlim(t_from_rot, t_to_rot)
plt.ylim(-1.5, 2.5)
plt.legend()
plt.grid()

plt.figure(7)
plt.plot(t2, v2_t, color='b', linestyle='-', label= 'blue Cube')
plt.plot(t2, vTrans_t, color='y', linestyle='-', label= 'comb. Body')
plt.xlabel("Time [s]")
plt.xticks(rotation='vertical')
plt.ylabel("Velocity [m/s]")
plt.title("x-Velocity over Time")
plt.xlim(t_from_rot, t_to_rot)
plt.ylim(-1, 5)
plt.legend()
plt.grid()

plt.figure(10)
plt.plot(t2, alpha_t, color='y', linestyle='-')
plt.xlabel("Time [s]")
plt.xticks(rotation='vertical')
plt.ylabel("Angle [rad]")
plt.title("Rotation Angle of comb. Body over Time (first Method)")
plt.xlim(t_from_rot, t_to_rot)
plt.ylim(-2, 8)
plt.grid()

plt.figure(11)
plt.plot(t2, vRot_t, color='y', linestyle='-')
plt.xlabel("Time [s]")
plt.xticks(rotation='vertical')
plt.ylabel("Angular Velocity [rad/s]")
plt.title("Angular x-Velocity over Time (first Method)")
plt.xlim(t_from_rot, t_to_rot)
plt.ylim(-0.5, 2)
plt.grid()

plt.figure(8)
plt.plot(t2, pRotTot_t, color='black', linestyle='-', label= 'Total')
plt.plot(t2, pIntrinsicRot_t, color='orange', linestyle='-', label= 'Intrinsic')
plt.plot(t2, pOrbitalRot_t, color='green', linestyle='-', label= 'Orbital')
plt.xlabel("Time [s]")
plt.xticks(rotation='vertical')
plt.ylabel("Anular Momentum [kg*m^2/s]")
plt.title("Angular Momentum of the comb. Body over Time")
plt.xlim(t_from_rot, t_to_rot)
plt.ylim(-75, 75)
plt.legend()
plt.grid()

eTransUndRot = np.array(eTrans_t) + np.array(eRot_t)

plt.figure(9)
plt.plot(t2, eTransUndRot, color='black', linestyle='-', label= 'E_trans + E_rot')
plt.plot(t2, eKin2_t, color='b', linestyle='-', label= 'E_trans blue Cube')
plt.plot(t2, eTrans_t, color='y', linestyle='-', label= 'E_trans comb. Body')
plt.plot(t2, eRot_t, color='g', linestyle='-', label= 'E_rot comb. Body')
plt.xlabel("Time [s]")
plt.xticks(rotation='vertical')
plt.ylabel("Energy [J]")
plt.title("Rotational and Translational Energy over Time")
plt.xlim(t_from_rot, t_to_rot)
plt.ylim(-3, 200)
plt.legend()
plt.grid()

plt.show()

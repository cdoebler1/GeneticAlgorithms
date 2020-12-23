# import sys
import numpy as np  # Numpy is Python's built in library for matrix operations.
from scipy.spatial import distance
# from pylab import *


def l2distance(X, Z=None):
    # function D=l2distance(X,Z)
    #
    # Computes the Euclidean distance matrix.
    # Syntax:
    # D=l2distance(X,Z)
    # Input:
    # X: nxd data matrix with n vectors (rows) of dimensionality d
    # Z: mxd data matrix with m vectors (rows) of dimensionality d
    #
    # Output:
    # Matrix D of size nxm
    # D(i,j) is the Euclidean distance of X(i,:) and Z(j,:)
    #
    # call with only one input:
    # l2distance(X)=l2distance(X,X)
    #
    if Z is None:
        Z = X

    n, d1 = X.shape
    m, d2 = Z.shape
    assert (d1 == d2), "Dimensions of input vectors must match!"

    # YOUR CODE HERE
    # S = np.inner(X, X)
    # R = np.inner(Z, Z)
    # S1 = np.add.outer(np.sum(X**2, axis=1), np.sum(Z**2, axis=1))
    # print(S, R, S1)
    P = np.add.outer(np.sum(X**2, axis=1), np.sum(Z**2, axis=1))
    G = np.inner(X, Z)
    D = np.sqrt(abs(P - 2*G))
    print(X)
    print(np.sort(X, 0))
    print(np.sort(X, 1))
    print(np.argsort(X, 1))
    # print(D)
    D = np.sort(D, 1)
    # D = distance.cdist(X, Z, 'euclidean')
    # print(D)
    return D


def test():
    X = np.array([[1, 2], [4, 3], [5, 6]])
    Z = np.array([[1, 0], [2, 1]])
    # X = np.random.rand(700, 100)
    D1 = l2distance(X, Z)  # compute distances from your solutions
    print(D1)
    # D2 = l2distance_grader(X, Z)  # compute distance from ground truth
    # print(D2)
    # test = np.linalg.norm(D1 - D2) # compare the two
    # return test<1e-5 # difference should be small


test()


    """
    function preds=knnclassifier(xTr,yTr,xTe,k);

    k-nn classifier

    Input:
    xTr = nxd input matrix with n row-vectors of dimensionality d
    xTe = mxd input matrix with m row-vectors of dimensionality d
    k = number of nearest neighbors to be found

    Output:

    preds = predicted labels, ie preds(i) is the predicted label of xTe(i,:)
    """
    # fix array shapes
    yTr = yTr.flatten()

    # YOUR CODE HERE
    P = np.add.outer(np.sum(xTr**2, axis=1), np.sum(xTe**2, axis=1))
    G = np.inner(xTr, xTe)
    dists = np.sqrt(abs(P - 2*G))
    indices = np.argsort(dists, 0)[0:k]
    dists = np.sort(dists, 0)[0:k]
    preds = mode(yTr[indices], axis=0)[0][0]
    return preds

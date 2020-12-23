import numpy as np


def naivebayesPXY(X, Y):
    """
    naivebayesPXY(X, Y) returns [posprob,negprob]

    Input:
        X : n input vectors of d dimensions (nxd)
        Y : n labels (-1 or +1) (n)

    Output:
        posprob: probability vector of p(x_alpha = 1|y=1)  (d)
        negprob: probability vector of p(x_alpha = 1|y=-1) (d)
    """

    n, d = X.shape
    X = np.concatenate([X, np.ones((2, d)), np.zeros((2, d))])
    Y = np.concatenate([Y, [-1, 1, -1, 1]])
    n, d = X.shape

    # YOUR CODE HERE
    Xpos = X[Y == 1]
    Xneg = X[Y == -1]
    npp, dp = Xpos.shape
    nn, dn = Xneg.shape
    posprob = Xpos.sum(0)/npp
    negprob = Xneg.sum(0)/nn
    return posprob, negprob


def naivebayesPXY_test1():
    x = np.array([[0, 1], [1, 0]])
    y = np.array([-1, 1])
#    print(x, y)
    pos, neg = naivebayesPXY(x, y)
    return pos, neg
#    pos0, neg0 = naivebayesPXY_grader(x, y)
#    test = np.linalg.norm(pos - pos0) + np.linalg.norm(neg - neg0)
#    return test < 1e-5


pos, neg = naivebayesPXY_test1()
print(pos, neg)

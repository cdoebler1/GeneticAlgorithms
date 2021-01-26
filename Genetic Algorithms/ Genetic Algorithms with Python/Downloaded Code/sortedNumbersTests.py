import datetime
import unittest

import genetic


def get_fitness(genes):
    fitness = 1
    for i in range(1, len(genes)):
        if genes[i] > genes[i - 1]:
            fitness += 1
    return fitness


def display(candidate, startTime):
    timeDiff = datetime.datetime.now() - startTime
    print("{}\t=> {}\t{}".format(
        ', '.join(map(str, candidate.Genes)),
        candidate.Fitness,
        timeDiff))


class SortedNumbersTests(unittest.TestCase):
    def test_sort_3_numbers(self):
        self.sort_numbers(3)

    def sort_numbers(self, totalNumbers):
        geneset = [i for i in range(100)]
        startTime = datetime.datetime.now()

        def fnDisplay(candidate):
            display(candidate, startTime)

        def fnGetFitness(genes):
            return get_fitness(genes)

        optimalFitness = totalNumbers
        best = genetic.get_best(fnGetFitness, totalNumbers, optimalFitness,
                                geneset, fnDisplay)
        self.assertTrue(not optimalFitness > best.Fitness)


if __name__ == '__main__':
    unittest.main()

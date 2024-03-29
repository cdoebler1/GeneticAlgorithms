#!/usr/bin/python3
# -*- coding:utf8 -*-


"""Produce new haiku from training corpus of existing haiku."""
import sys
import logging
import random
import json
from flask import Flask, request, make_response, jsonify
from collections import defaultdict
from count_syllables import count_syllables
from string import punctuation

logging.disable(logging.CRITICAL)  # comment-out to enable debugging messages
logging.basicConfig(level=logging.DEBUG, format='%(message)s')


def load_training_file():
    """Return a text file as a string."""
    with open('Training.json') as f_obj:
        files = json.load(f_obj)
    raw_haiku = ''
    for file in files:
        file = file + '.txt'
        with open(file) as f:
            raw_haiku = raw_haiku + f.read()
    return raw_haiku


def prep_training(raw_haiku):
    """Load string, remove newline, split words on spaces, and return list."""
    # corpus = raw_haiku.replace('\n', ' ').strip(punctuation).lower().split()
    corpus = raw_haiku.replace('\n', ' ').replace('"', '')
    corpus = corpus.lower().replace(',', '').replace('.', '').split()
    return corpus


def map_word_to_word(corpus):
    """Load list & use dictionary to map word to word that follows."""
    limit = len(corpus)-1
    dict1_to_1 = defaultdict(list)
    for index, word in enumerate(corpus):
        if index < limit:
            suffix = corpus[index + 1]
            dict1_to_1[word].append(suffix)
    logging.debug("map_word_to_word results for \"sake\" = %s\n",
                  dict1_to_1['sake'])
    return dict1_to_1


def map_2_words_to_word(corpus):
    """Load list & use dictionary to map word-pair to trailing word."""
    limit = len(corpus)-2
    dict2_to_1 = defaultdict(list)
    for index, word in enumerate(corpus):
        if index < limit:
            key = word + ' ' + corpus[index + 1]
            suffix = corpus[index + 2]
            dict2_to_1[key].append(suffix)
    logging.debug("map_2_words_to_word results for \"sake jug\" = %s\n",
                  dict2_to_1['sake jug'])
    return dict2_to_1


def random_word(corpus):
    """Return random word and syllable count from training corpus."""
    word = random.choice(corpus)
    num_syls = count_syllables(word)
    if num_syls > 4:
        random_word(corpus)
    else:
        logging.debug("random word & syllables = %s %s\n", word, num_syls)
        return (word, num_syls)


def word_after_single(prefix, suffix_map_1, current_syls, target_syls):
    """Return all acceptable words in a corpus that follow a single word."""
    accepted_words = []
    suffixes = suffix_map_1.get(prefix)
    if suffixes is not None:
        for candidate in suffixes:
            num_syls = count_syllables(candidate)
            if current_syls + num_syls <= target_syls:
                accepted_words.append(candidate)
    logging.debug("accepted words after \"%s\" = %s\n",
                  prefix, set(accepted_words))
    return accepted_words


def word_after_double(prefix, suffix_map_2, current_syls, target_syls):
    """Return all acceptable words in a corpus that follow a word pair."""
    accepted_words = []
    suffixes = suffix_map_2.get(prefix)
    if suffixes is not None:
        for candidate in suffixes:
            num_syls = count_syllables(candidate)
            if current_syls + num_syls <= target_syls:
                accepted_words.append(candidate)
    logging.debug("accepted words after \"%s\" = %s\n",
                  prefix, set(accepted_words))
    return accepted_words


def haiku_line(suffix_map_1, suffix_map_2, corpus, end_prev_line, target_syls):
    """Build a haiku line from a training corpus and return it."""
    line = '2/3'
    line_syls = 0
    current_line = []

    if len(end_prev_line) == 0:  # build first line
        line = '1'
        word, num_syls = random_word(corpus)
        current_line.append(word)
        line_syls += num_syls
        word_choices = word_after_single(word, suffix_map_1,
                                         line_syls, target_syls)
        while len(word_choices) == 0:
            prefix = random.choice(corpus)
            logging.debug("new random prefix = %s", prefix)
            word_choices = word_after_single(prefix, suffix_map_1,
                                             line_syls, target_syls)
        word = random.choice(word_choices)
        num_syls = count_syllables(word)
        logging.debug("word & syllables = %s %s", word, num_syls)
        line_syls += num_syls
        current_line.append(word)
        if line_syls == target_syls:
            end_prev_line.extend(current_line[-2:])
            return current_line, end_prev_line

    else:  # build lines 2 & 3
        current_line.extend(end_prev_line)

    while True:
        logging.debug("line = %s\n", line)
        prefix = current_line[-2] + ' ' + current_line[-1]
        word_choices = word_after_double(prefix, suffix_map_2,
                                         line_syls, target_syls)
        while len(word_choices) == 0:
            index = random.randint(0, len(corpus) - 2)
            prefix = corpus[index] + ' ' + corpus[index + 1]
            logging.debug("new random prefix = %s", prefix)
            word_choices = word_after_double(prefix, suffix_map_2,
                                             line_syls, target_syls)
        word = random.choice(word_choices)
        num_syls = count_syllables(word)
        logging.debug("word & syllables = %s %s", word, num_syls)

        if line_syls + num_syls > target_syls:
            continue
        elif line_syls + num_syls < target_syls:
            current_line.append(word)
            line_syls += num_syls
        elif line_syls + num_syls == target_syls:
            current_line.append(word)
            break

    end_prev_line = []
    end_prev_line.extend(current_line[-2:])

    if line == '1':
        final_line = current_line[:]
    else:
        final_line = current_line[2:]

    return final_line, end_prev_line


app = Flask("Haiku Action")


def respond(fulfillment):
    return make_response(jsonify({'fulfillmentText': fulfillment}))


@app.route('/departures', methods=['POST'])
def departures_handler():
    return respond(haiku)

def main():
    """Give user choice of building a haiku or modifying an existing haiku."""

    raw_haiku = load_training_file()
    corpus = prep_training(raw_haiku)
    suffix_map_1 = map_word_to_word(corpus)
    suffix_map_2 = map_2_words_to_word(corpus)
    final = []

    end_prev_line = []
    first_line, end_prev_line1 = haiku_line(suffix_map_1, suffix_map_2,
                                            corpus, end_prev_line, 5)
    final.append(first_line)
    line, end_prev_line2 = haiku_line(suffix_map_1, suffix_map_2,
                                      corpus, end_prev_line1, 7)
    final.append(line)
    line, end_prev_line3 = haiku_line(suffix_map_1, suffix_map_2,
                                      corpus, end_prev_line2, 5)
    final.append(line)

    # display results
    print()
    print(" ".join(final[0]), file=sys.stderr)
    print(" ".join(final[1]), file=sys.stderr)
    print(" ".join(final[2]), file=sys.stderr)
    print()

    haiku = " ".join(final[0]) + " " + " ".join(final[1]) + " " + " ".join(final[2])

    app.run()


if __name__ == '__main__':
    main()

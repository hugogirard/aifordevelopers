// Copyright (c) Microsoft. All rights reserved.

import * as speechSdk from 'microsoft-cognitiveservices-speech-sdk';
import { BaseService } from './BaseService';
import { currentLanguage } from "../../language/languageContext";

interface TokenResponse {
    token: string;
    region: string;
    isSuccess: boolean;
}

export class SpeechService extends BaseService {
    getSpeechTokenAsync = async (accessToken: string): Promise<TokenResponse> => {
        const result = await this.getResponseAsync<TokenResponse>(
            {
                commandPath: 'speechToken',
                method: 'GET',
            },
            accessToken,
        );

        return result;
    };

    getSpeechRecognizerAsyncWithValidKey = (response: TokenResponse) => {
        const { token, region, isSuccess } = response;

        if (isSuccess) {
            return this.generateSpeechRecognizer(token, region);
        }

        return undefined;
    };

    getSpeechSynthesizerAsyncWithValidKey = (response: TokenResponse) => {
        const { token, region, isSuccess } = response;

        if (isSuccess) {
            return this.generateSpeechSynthesizer(token, region);
        }

        return undefined;
    };    

    private generateSpeechRecognizer(token: string, region: string) {
        const speechConfig = speechSdk.SpeechConfig.fromAuthorizationToken(token, region);        
        speechConfig.speechRecognitionLanguage = currentLanguage == "en" ? "en-US" : "fr-CA";
        const audioConfig = speechSdk.AudioConfig.fromDefaultMicrophoneInput();
        return new speechSdk.SpeechRecognizer(speechConfig, audioConfig);
    }

    private generateSpeechSynthesizer(token: string, region: string) {
        const speechConfig = speechSdk.SpeechConfig.fromAuthorizationToken(token, region);
        speechConfig.speechSynthesisLanguage = currentLanguage == "en" ? "en-US" : "fr-CA";
        const audioConfig = speechSdk.AudioConfig.fromDefaultSpeakerOutput();
        return new speechSdk.SpeechSynthesizer(speechConfig, audioConfig);
    }    
}

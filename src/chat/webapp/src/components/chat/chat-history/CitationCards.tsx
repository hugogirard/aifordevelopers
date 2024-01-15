// Copyright (c) Microsoft. All rights reserved.

import {
    Badge,
    Caption1,
    Card,
    CardHeader,
    makeStyles,
    shorthands
} from '@fluentui/react-components';
import { IChatMessage } from '../../../libs/models/ChatMessage';
import { customTokens } from '../../../styles';

const useClasses = makeStyles({
    root: {
        display: 'flex',
        ...shorthands.gap(customTokens.spacingVerticalS),
        flexDirection: 'column',
    },
    card: {
        display: 'flex',
        width: '100%',
        height: 'fit-content',
    },
});

interface ICitationCardsProps {
    message: IChatMessage;
}

export const CitationCards: React.FC<ICitationCardsProps> = ({ message }) => {
    const classes = useClasses();

    let BackendServiceUrl =
    process.env.REACT_APP_BACKEND_URI == null || process.env.REACT_APP_BACKEND_URI.trim() === ''
        ? window.origin
        : process.env.REACT_APP_BACKEND_URI;
    if (!BackendServiceUrl.endsWith('/')) BackendServiceUrl += '/';

    if (!message.citations || message.citations.length === 0) {
        return null;
    }

    return (
        <div className={classes.root}>
            {message.citations.map((citation, index) => {
                return (
                    <Card className={classes.card} size="small" key={`citation-card-${index}`}>
                        <CardHeader
                            image={
                                <Badge shape="rounded" appearance="outline" color="informative">
                                    {index + 1}
                                </Badge>
                            }
                            header={<a target="_blank" rel="noreferrer" href={`${BackendServiceUrl}documents/${citation.link}`}>{citation.link}</a>}
                            description={<Caption1>Relevance score: {citation.relevanceScore.toFixed(3)}</Caption1>}
                        />
                    </Card>
                );
            })}
        </div>
    );
};
